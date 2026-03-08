import { useState, useCallback } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { formatDistanceToNow, format, isPast } from "date-fns";
import { ArrowLeft, Clock, Gavel, Eye, EyeOff, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent } from "@/components/ui/card";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { useAuth } from "@/shared/hooks/useAuth";
import { auctionsApi } from "./api";
import { listingsApi } from "@/features/listings/api";
import { useAuctionHub } from "./useAuctionHub";
import type { AuctionBidDto } from "@/shared/types/common";
import type { BidPlacedMessage, AuctionClosedMessage } from "./useAuctionHub";
import { createAuctionCheckout, getPaymentByListing } from "../payments/api";

export function AuctionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const queryClient = useQueryClient();

  const [bidAmount, setBidAmount] = useState("");
  const [bidError, setBidError] = useState("");
  const [liveBids, setLiveBids] = useState<AuctionBidDto[]>([]);
  const [liveClosed, setLiveClosed] = useState(false);
  const [liveWinnerId, setLiveWinnerId] = useState<string | null>(null);
  const [liveFinalAmount, setLiveFinalAmount] = useState<number | null>(null);
  const [liveClosedAt, setLiveClosedAt] = useState<string | null>(null);
  const [liveEndTime, setLiveEndTime] = useState<string | null>(null);
  const [activeImage, setActiveImage] = useState(0);

  const { data: auction, isLoading } = useQuery({
    queryKey: ["auction", id],
    queryFn: () => auctionsApi.getAuction(id!),
    enabled: !!id,
  });

  const { data: listing } = useQuery({
    queryKey: ["listing", auction?.listingId],
    queryFn: () => listingsApi.getListing(auction!.listingId),
    enabled: !!auction?.listingId,
  });
  const { data: isWatchedData } = useQuery({
    queryKey: ["listing-watched", auction?.listingId],
    queryFn: () => listingsApi.isWatched(auction!.listingId),
    enabled: !!auction?.listingId && isAuthenticated,
  });
  const watchlisted = isWatchedData ?? false;

  const { data: winnerPayment } = useQuery({
    queryKey: ["payment-listing", auction?.listingId],
    queryFn: () => getPaymentByListing(auction!.listingId),
    enabled:
      !!auction?.listingId && (auction?.status === "Closed" || liveClosed),
  });

  const onBidPlaced = useCallback((msg: BidPlacedMessage) => {
    setLiveBids((prev) => [
      {
        id: crypto.randomUUID(),
        bidderId: msg.bidderId,
        bidderName: msg.bidderName,
        amount: msg.amount,
        createdAt: new Date().toISOString(),
      },
      ...prev,
    ]);
    setLiveEndTime(msg.newEndTime);
  }, []);
  const onAuctionClosed = useCallback(
    (msg: AuctionClosedMessage) => {
      setLiveClosed(true);
      if (msg.winnerId) setLiveWinnerId(msg.winnerId);
      if (msg.finalAmount) setLiveFinalAmount(msg.finalAmount);
      if (msg.closedAt) setLiveClosedAt(msg.closedAt);
      queryClient.invalidateQueries({ queryKey: ["auction", id] });
    },
    [id, queryClient],
  );

  const onAuctionStarted = useCallback(() => {
    queryClient.invalidateQueries({ queryKey: ["auction", id] });
  }, [id, queryClient]);

  useAuctionHub({
    auctionId: id ?? "",
    onBidPlaced,
    onAuctionClosed,
    onAuctionStarted,
  });

  const placeBidMutation = useMutation({
    mutationFn: (amount: number) => auctionsApi.placeBid(id!, { amount }),
    onSuccess: () => {
      setBidAmount("");
      setBidError("");
    },
    onError: (err: any) => {
      setBidError(err.response?.data?.detail ?? "Failed to place bid.");
    },
  });

  const closeMutation = useMutation({
    mutationFn: () => auctionsApi.closeAuction(id!),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["auction", id] }),
  });
  const watchMutation = useMutation({
    mutationFn: () =>
      watchlisted
        ? listingsApi.removeFromWatchlist(auction!.listingId)
        : listingsApi.addToWatchlist(auction!.listingId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["listing-watched", auction!.listingId],
      });
      queryClient.invalidateQueries({ queryKey: ["watchlist", "listings"] });
      queryClient.invalidateQueries({ queryKey: ["listings-watchlist"] });
    },
  });

  if (isLoading) return <LoadingSpinner />;
  if (!auction)
    return (
      <div className="py-24 text-center text-gray-400">Auction not found.</div>
    );

  const status = liveClosed ? "Closed" : auction.status;

  const endTime =
    liveClosedAt ?? auction.closedAt ?? liveEndTime ?? auction.endTime;
  const auctionBidsReversed = [...(auction.bids ?? [])].reverse();
  const bids = liveBids.length ? liveBids : auctionBidsReversed;
  const currentBid = liveFinalAmount
    ? liveFinalAmount
    : bids.length
      ? Math.max(...bids.map((b) => b.amount))
      : (auction.currentHighestBid ?? auction.startingPrice);

  const currentBidderId =
    liveWinnerId ??
    (bids.length
      ? bids.reduce((a, b) => (a.amount > b.amount ? a : b)).bidderId
      : auction.currentHighestBidderId);

  const isWinner =
    (liveClosed || status === "Closed") && currentBidderId === user?.id;

  const isSeller = user?.id === auction.sellerId;

  const canBid = isAuthenticated && !isSeller && status === "Active";
  const canClose = isSeller && status === "Active";
  const canWatch =
    isAuthenticated &&
    !isSeller &&
    (status === "Scheduled" || status === "Active");
  const isEnded = isPast(new Date(endTime));

  function handlePlaceBid() {
    const amount = parseFloat(bidAmount);
    if (isNaN(amount) || amount <= 0) {
      setBidError("Enter a valid amount.");
      return;
    }
    if (amount <= currentBid) {
      setBidError(`Bid must be higher than $${currentBid.toFixed(2)}.`);
      return;
    }
    setBidError("");
    placeBidMutation.mutate(amount);
  }

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-800 transition-colors"
      >
        <ArrowLeft className="w-4 h-4" />
        Back
      </button>

      <div className="flex items-center gap-3">
        <Link
          to={`/listings/${auction.listingId}`}
          className="text-lg font-semibold hover:underline"
        >
          {auction.listingTitle}
        </Link>
        <StatusBadge status={status} />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-[5fr_4fr] gap-8 items-start">
        {/* Left — image gallery */}
        {listing?.images?.length ? (
          <div className="space-y-2">
            <div className="w-full aspect-[4/3] rounded-lg overflow-hidden bg-gray-100">
              <img
                src={listing.images[activeImage].url}
                alt={auction.listingTitle}
                className="w-full h-full object-contain"
              />
            </div>
            {listing.images.length > 1 && (
              <div className="flex gap-2 overflow-x-auto pb-1">
                {listing.images.map((img, i) => (
                  <button
                    key={img.displayOrder}
                    onClick={() => setActiveImage(i)}
                    className={`shrink-0 w-16 h-16 rounded overflow-hidden border-2 transition-colors ${
                      i === activeImage
                        ? "border-orange-500"
                        : "border-transparent"
                    }`}
                  >
                    <img
                      src={img.url}
                      alt=""
                      className="w-full h-full object-cover"
                    />
                  </button>
                ))}
              </div>
            )}
          </div>
        ) : (
          <div className="aspect-[4/3] rounded-lg bg-gray-100" />
        )}

        {/* Right — bid panel + bid history */}
        <div className="space-y-4">
          <Card>
            <CardContent className="p-6 space-y-5">
              <div>
                <p className="text-sm text-gray-500">
                  {bids.length ? "Current bid" : "Starting price"}
                </p>
                <p className="text-3xl font-bold">${currentBid.toFixed(2)}</p>
                {bids.length > 0 && (
                  <p className="text-sm text-gray-500 mt-0.5">
                    {bids.length} bid{bids.length !== 1 ? "s" : ""}
                  </p>
                )}
              </div>

              {(status === "Closed" || liveClosed) &&
                currentBidderId &&
                isSeller && (
                  <p className="text-sm text-gray-600">
                    Winner:{" "}
                    <Link
                      to={`/users/${currentBidderId}`}
                      className="font-medium text-orange-500 hover:underline"
                    >
                      {liveBids.find((b) => b.bidderId === currentBidderId)
                        ?.bidderName ??
                        auction.bids.find((b) => b.bidderId === currentBidderId)
                          ?.bidderName ??
                        "View profile"}
                    </Link>
                  </p>
                )}

              <div className="flex items-center gap-2 text-sm text-gray-600">
                <Clock className="w-4 h-4" />
                {status === "Scheduled" && (
                  <span>
                    Starts{" "}
                    {formatDistanceToNow(new Date(auction.startTime), {
                      addSuffix: true,
                    })}
                  </span>
                )}
                {status === "Active" && !isEnded && (
                  <span>
                    Ends{" "}
                    {formatDistanceToNow(new Date(endTime), {
                      addSuffix: true,
                    })}
                  </span>
                )}
                {(status === "Closed" || isEnded) && (
                  <span>
                    Ended {format(new Date(endTime), "d MMM yyyy, h:mm a")}
                  </span>
                )}
              </div>

              <div className="text-xs text-gray-400 space-y-0.5">
                <p>
                  Start:{" "}
                  {format(new Date(auction.startTime), "d MMM yyyy, h:mm a")}
                </p>
                <p>End: {format(new Date(endTime), "d MMM yyyy, h:mm a")}</p>
              </div>

              {canBid && (
                <div className="space-y-2">
                  <div className="flex gap-2">
                    <Input
                      type="number"
                      placeholder={`More than $${currentBid.toFixed(2)}`}
                      value={bidAmount}
                      onChange={(e) => setBidAmount(e.target.value)}
                      className="flex-1"
                    />
                    <Button
                      onClick={handlePlaceBid}
                      disabled={placeBidMutation.isPending}
                      className="bg-[#1a1a1a] hover:bg-gray-800 text-white"
                    >
                      <Gavel className="w-4 h-4 mr-1.5" />
                      Bid
                    </Button>
                  </div>
                  {bidError && (
                    <p className="text-sm text-red-500">{bidError}</p>
                  )}
                </div>
              )}

              {!isAuthenticated && status === "Active" && (
                <p className="text-sm text-gray-500">
                  <Link to="/login" className="text-orange-500 hover:underline">
                    Sign in
                  </Link>{" "}
                  to place a bid.
                </p>
              )}

              {isWinner && winnerPayment?.status !== "Completed" && (
                <div className="rounded-md bg-green-50 border border-green-200 p-3 space-y-3">
                  <p className="text-sm text-green-700 font-medium">
                    You won this auction! 🎉
                  </p>
                  <Button
                    className="w-full bg-orange-500 hover:bg-orange-600 text-white"
                    onClick={async () => {
                      try {
                        const { checkoutUrl } = await createAuctionCheckout({
                          auctionId: auction.id,
                        });
                        window.location.href = checkoutUrl;
                      } catch {}
                    }}
                  >
                    Pay Now
                  </Button>
                </div>
              )}
              {isSeller &&
                (status === "Closed" || liveClosed) &&
                winnerPayment && (
                  <div
                    className={`rounded-md p-3 text-sm font-medium ${
                      winnerPayment.status === "Completed"
                        ? "bg-green-50 border border-green-200 text-green-700"
                        : "bg-orange-50 border border-orange-200 text-orange-700"
                    }`}
                  >
                    {winnerPayment.status === "Completed"
                      ? "Payment received"
                      : "Awaiting payment from winner"}
                  </div>
                )}
              {canClose && (
                <Button
                  variant="outline"
                  className="w-full border-red-300 text-red-600 hover:bg-red-50"
                  onClick={() => closeMutation.mutate()}
                  disabled={closeMutation.isPending}
                >
                  <X className="w-4 h-4 mr-1.5" />
                  Close Auction
                </Button>
              )}

              {canWatch && (
                <Button
                  variant="outline"
                  className="w-full"
                  onClick={() => watchMutation.mutate()}
                  disabled={watchMutation.isPending}
                >
                  {watchlisted ? (
                    <>
                      <EyeOff className="w-4 h-4 mr-1.5" />
                      Unwatch
                    </>
                  ) : (
                    <>
                      <Eye className="w-4 h-4 mr-1.5" />
                      Watch
                    </>
                  )}
                </Button>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardContent className="p-6">
              <h3 className="font-semibold mb-4">Bid history</h3>
              {bids.length === 0 ? (
                <p className="text-sm text-gray-400">No bids yet.</p>
              ) : (
                <div className="max-h-64 overflow-y-auto">
                  <ul className="space-y-3">
                    {bids.map((bid) => (
                      <li
                        key={bid.id}
                        className="flex items-center justify-between text-sm"
                      >
                        <Link
                          to={`/users/${bid.bidderId}`}
                          className="font-medium hover:underline hover:text-orange-500 transition-colors"
                        >
                          {bid.bidderName}
                        </Link>
                        <span className="text-gray-500">
                          ${bid.amount.toFixed(2)}
                        </span>
                        <span className="text-gray-400 text-xs">
                          {formatDistanceToNow(new Date(bid.createdAt), {
                            addSuffix: true,
                          })}
                        </span>
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
