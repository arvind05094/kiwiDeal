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
import { useAuctionHub } from "./useAuctionHub";
import type { AuctionDto, AuctionBidDto } from "@/shared/types/common";
import type { BidPlacedMessage, AuctionClosedMessage } from "./useAuctionHub";

export function AuctionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const queryClient = useQueryClient();

  const [bidAmount, setBidAmount] = useState("");
  const [bidError, setBidError] = useState("");
  const [liveEndTime, setLiveEndTime] = useState<string | null>(null);
  const [liveBids, setLiveBids] = useState<AuctionBidDto[]>([]);
  const [liveClosed, setLiveClosed] = useState(false);
  const [watchlisted, setWatchlisted] = useState(false);

  const { data: auction, isLoading } = useQuery({
    queryKey: ["auction", id],
    queryFn: () => auctionsApi.getAuction(id!),
    enabled: !!id,
    onSuccess: (data: AuctionDto) => {
      setLiveBids(data.bids ?? []);
      setLiveEndTime(data.endTime);
    },
  });

  const onBidPlaced = useCallback(
    (msg: BidPlacedMessage) => {
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
      queryClient.invalidateQueries({ queryKey: ["auction", id] });
    },
    [id, queryClient],
  );

  const onAuctionClosed = useCallback(
    (_msg: AuctionClosedMessage) => {
      setLiveClosed(true);
      queryClient.invalidateQueries({ queryKey: ["auction", id] });
    },
    [id, queryClient],
  );

  useAuctionHub({
    auctionId: id ?? "",
    onBidPlaced,
    onAuctionClosed,
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
        ? auctionsApi.removeFromWatchlist(id!)
        : auctionsApi.addToWatchlist(id!),
    onSuccess: () => setWatchlisted((w) => !w),
  });

  if (isLoading) return <LoadingSpinner />;
  if (!auction)
    return (
      <div className="py-24 text-center text-gray-400">Auction not found.</div>
    );

  const status = liveClosed ? "Closed" : auction.status;
  const endTime = liveEndTime ?? auction.endTime;
  const bids = liveBids.length ? liveBids : (auction.bids ?? []);
  const currentBid =
    bids[0]?.amount ?? auction.currentHighestBid ?? auction.startingPrice;
  const currentBidderId = bids[0]?.bidderId ?? auction.currentHighestBidderId;
  const isSeller = user?.id === auction.sellerId;
  const isWinner = status === "Closed" && currentBidderId === user?.id;
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
    <div className="max-w-4xl mx-auto space-y-6">
      {/* Back */}
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-800 transition-colors"
      >
        <ArrowLeft className="w-4 h-4" />
        Back
      </button>

      {/* Listing context */}
      <div className="flex items-center gap-3">
        <Link
          to={`/listings/${auction.listingId}`}
          className="text-lg font-semibold hover:underline"
        >
          {auction.listingTitle}
        </Link>
        <StatusBadge status={status} />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Left — bid info */}
        <Card>
          <CardContent className="p-6 space-y-5">
            {/* Current bid */}
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

            {/* Timer */}
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
                  {formatDistanceToNow(new Date(endTime), { addSuffix: true })}
                </span>
              )}
              {(status === "Closed" || isEnded) && (
                <span>
                  Ended {format(new Date(endTime), "d MMM yyyy, h:mm a")}
                </span>
              )}
            </div>

            {/* Start / end times */}
            <div className="text-xs text-gray-400 space-y-0.5">
              <p>
                Start:{" "}
                {format(new Date(auction.startTime), "d MMM yyyy, h:mm a")}
              </p>
              <p>End: {format(new Date(endTime), "d MMM yyyy, h:mm a")}</p>
            </div>

            {/* Place bid */}
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
                {bidError && <p className="text-sm text-red-500">{bidError}</p>}
              </div>
            )}

            {/* Guest prompt */}
            {!isAuthenticated && status === "Active" && (
              <p className="text-sm text-gray-500">
                <Link to="/login" className="text-orange-500 hover:underline">
                  Sign in
                </Link>{" "}
                to place a bid.
              </p>
            )}

            {/* Winner */}
            {isWinner && (
              <div className="rounded-md bg-green-50 border border-green-200 p-3 text-sm text-green-700 font-medium">
                You won this auction! 🎉
              </div>
            )}

            {/* Seller controls */}
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

            {/* Watch */}
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

        {/* Right — bid history */}
        <Card>
          <CardContent className="p-6">
            <h3 className="font-semibold mb-4">Bid history</h3>
            {bids.length === 0 ? (
              <p className="text-sm text-gray-400">No bids yet.</p>
            ) : (
              <ul className="space-y-3">
                {bids.map((bid) => (
                  <li
                    key={bid.id}
                    className="flex items-center justify-between text-sm"
                  >
                    <span className="font-medium">{bid.bidderName}</span>
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
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
