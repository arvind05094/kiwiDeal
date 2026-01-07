import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useAuth } from "@/features/auth/AuthContext";
import { auctionsApi } from "./api";
import { useAuctionHub } from "./hooks/useAuctionHub";
import { AuctionDto, BidDto } from "./types";
import Navbar from "@/shared/components/Navbar";
import { paymentsApi } from "../payments/api";

export default function AuctionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [bidAmount, setBidAmount] = useState("");
  const [bidError, setBidError] = useState("");
  const [closeError, setCloseError] = useState("");

  const {
    data: auction,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["auction", id],
    queryFn: () => auctionsApi.getAuction(id!).then((r) => r.data),
    enabled: !!id,
  });

  useAuctionHub({
    auctionId: id!,
    onBidPlaced: (event) => {
      queryClient.setQueryData<AuctionDto>(["auction", id], (old) => {
        if (!old) return old;
        const newBid: BidDto = {
          id: crypto.randomUUID(),
          bidderId: event.bidderId,
          amount: event.amount,
          createdAt: new Date().toISOString(),
        };
        return {
          ...old,
          currentHighestBid: event.amount,
          currentHighestBidderId: event.bidderId,
          endTime: event.newEndTime,
          bids: [newBid, ...old.bids],
        };
      });
    },
  });

  const placeBid = useMutation({
    mutationFn: () =>
      auctionsApi.placeBid(id!, { amount: parseFloat(bidAmount) }),
    onSuccess: () => {
      setBidAmount("");
      setBidError("");
      queryClient.invalidateQueries({ queryKey: ["auction", id] });
    },
    onError: (err: unknown) => {
      const msg = (err as { response?: { data?: { detail?: string } } })
        ?.response?.data?.detail;
      setBidError(msg ?? "Failed to place bid.");
    },
  });

  const closeAuction = useMutation({
    mutationFn: () => auctionsApi.closeAuction(id!),
    onSuccess: () => {
      setCloseError("");
      queryClient.invalidateQueries({ queryKey: ["auction", id] });
    },
    onError: () => setCloseError("Failed to close auction."),
  });

  const checkout = useMutation({
    mutationFn: () => paymentsApi.checkout({ auctionId: id! }),
    onSuccess: (res) => {
      window.location.href = res.data.checkoutUrl;
    },
  });

  if (isLoading)
    return <div className="p-8 text-center">Loading auction...</div>;
  if (isError || !auction)
    return (
      <div className="p-8 text-center text-red-500">Auction not found.</div>
    );

  const isSeller = user?.id === auction.sellerId;
  const canBid = auction.status === "Active" && !isSeller;
  const canClose = auction.status === "Active" && isSeller;

  const isWinner =
    auction.status === "Closed" && user?.id === auction.currentHighestBidderId;

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <div className="max-w-2xl mx-auto p-6 space-y-6">
        {/* Header */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex justify-between items-start">
            <div>
              <p className="text-sm text-gray-500">
                Listing ID: {auction.listingId}
              </p>
              <p className="mt-2 text-2xl font-bold">
                {auction.currentHighestBid != null
                  ? `$${auction.currentHighestBid.toFixed(2)}`
                  : `$${auction.startingPrice.toFixed(2)}`}
              </p>
              <p className="text-sm text-gray-500 mt-1">
                {auction.currentHighestBid != null
                  ? "Current highest bid"
                  : "Starting price"}
              </p>
            </div>
            <span
              className={`text-xs font-medium px-2 py-1 rounded-full ${
                auction.status === "Active"
                  ? "bg-green-100 text-green-700"
                  : auction.status === "Scheduled"
                    ? "bg-yellow-100 text-yellow-700"
                    : "bg-gray-100 text-gray-600"
              }`}
            >
              {auction.status}
            </span>
          </div>
          <div className="mt-4 text-sm text-gray-600 space-y-1">
            <p>Starts: {new Date(auction.startTime).toLocaleString()}</p>
            <p>Ends: {new Date(auction.endTime).toLocaleString()}</p>
          </div>
        </div>

        {/* Place Bid */}
        {canBid && (
          <div className="bg-white rounded-lg shadow p-6">
            <h2 className="text-lg font-semibold mb-3">Place a Bid</h2>
            <div className="flex gap-3">
              <input
                type="number"
                min={0}
                step={0.01}
                value={bidAmount}
                onChange={(e) => setBidAmount(e.target.value)}
                placeholder="Enter amount"
                className="border rounded px-3 py-2 flex-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <button
                onClick={() => placeBid.mutate()}
                disabled={!bidAmount || placeBid.isPending}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
              >
                {placeBid.isPending ? "Placing..." : "Bid"}
              </button>
            </div>
            {bidError && (
              <p className="text-red-500 text-sm mt-2">{bidError}</p>
            )}
          </div>
        )}

        {/* Close Auction */}
        {canClose && (
          <div className="bg-white rounded-lg shadow p-6">
            <h2 className="text-lg font-semibold mb-3">Close Auction</h2>
            <button
              onClick={() => closeAuction.mutate()}
              disabled={closeAuction.isPending}
              className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700 disabled:opacity-50"
            >
              {closeAuction.isPending ? "Closing..." : "Close Auction"}
            </button>
            {closeError && (
              <p className="text-red-500 text-sm mt-2">{closeError}</p>
            )}
          </div>
        )}

        {isWinner && (
          <div className="bg-white rounded-lg shadow p-6">
            <h2 className="text-lg font-semibold mb-3">
              You won this auction!
            </h2>
            <button
              onClick={() => checkout.mutate()}
              disabled={checkout.isPending}
              className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 disabled:opacity-50"
            >
              {checkout.isPending ? "Redirecting..." : "Pay Now"}
            </button>
            {checkout.isError && (
              <p className="text-red-500 text-sm mt-2">
                Failed to start checkout.
              </p>
            )}
            <button
              onClick={() => navigate(`/payments/${id}`)}
              className="ml-3 text-sm text-blue-600 underline"
            >
              View payment status
            </button>
          </div>
        )}

        {/* Bid Feed */}
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-lg font-semibold mb-3">Bid History</h2>
          {auction.bids.length === 0 ? (
            <p className="text-gray-500 text-sm">No bids yet.</p>
          ) : (
            <ul className="space-y-2">
              {auction.bids.map((bid) => (
                <li
                  key={bid.id}
                  className="flex justify-between text-sm border-b pb-2 last:border-0"
                >
                  <span className="text-gray-600 truncate">{bid.bidderId}</span>
                  <span className="font-semibold">
                    ${bid.amount.toFixed(2)}
                  </span>
                  <span className="text-gray-400">
                    {new Date(bid.createdAt).toLocaleTimeString()}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
