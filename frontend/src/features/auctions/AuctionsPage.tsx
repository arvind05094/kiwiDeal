import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { auctionsApi } from "./api";
import Navbar from "@/shared/components/Navbar";
import { AuctionDto } from "./types";

export default function AuctionsPage() {
  const [page, setPage] = useState(1);

  const { data, isLoading, isError } = useQuery({
    queryKey: ["auctions", page],
    queryFn: () => auctionsApi.getAuctions(page, 10).then((r) => r.data),
  });

  if (isLoading)
    return <div className="p-8 text-center">Loading auctions...</div>;
  if (isError)
    return (
      <div className="p-8 text-center text-red-500">
        Failed to load auctions.
      </div>
    );

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <div className="max-w-4xl mx-auto p-6">
        <h1 className="text-2xl font-bold mb-6">Auctions</h1>

        {data?.items.length === 0 && (
          <p className="text-gray-500">No auctions available.</p>
        )}

        <div className="grid gap-4">
          {data?.items.map((auction: AuctionDto) => (
            <Link
              key={auction.id}
              to={`/auctions/${auction.id}`}
              className="block bg-white rounded-lg shadow p-4 hover:shadow-md transition"
            >
              <div className="flex justify-between items-center">
                <div>
                  <p className="font-medium">{auction.listingTitle}</p>
                  <p className="mt-1 text-sm text-gray-600">
                    {auction.currentHighestBid != null
                      ? `Current bid: $${auction.currentHighestBid.toFixed(2)}`
                      : `Starting at $${auction.startingPrice.toFixed(2)}`}
                  </p>
                  <p className="text-sm text-gray-500">
                    Ends: {new Date(auction.endTime).toLocaleString()}
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
            </Link>
          ))}
        </div>

        {data && data.totalCount > 10 && (
          <div className="flex justify-center gap-4 mt-6">
            <button
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 bg-white border rounded disabled:opacity-50"
            >
              Previous
            </button>
            <span className="self-center text-sm text-gray-600">
              Page {page}
            </span>
            <button
              onClick={() => setPage((p) => p + 1)}
              disabled={page * 10 >= data.totalCount}
              className="px-4 py-2 bg-white border rounded disabled:opacity-50"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
