import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { auctionsApi } from "./api";

interface Props {
  listingId: string;
  startingPrice: number;
  onClose: () => void;
}

export default function CreateAuctionModal({
  listingId,
  startingPrice,
  onClose,
}: Props) {
  const queryClient = useQueryClient();
  const [startTime, setStartTime] = useState("");
  const [endTime, setEndTime] = useState("");
  const [error, setError] = useState("");

  const createAuction = useMutation({
    mutationFn: () =>
      auctionsApi.createAuction({
        listingId,
        startingPrice,
        startTime: new Date(startTime).toISOString(),
        endTime: new Date(endTime).toISOString(),
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listing", listingId] });
      onClose();
    },
    onError: () => setError("Failed to create auction."),
  });

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-md space-y-4">
        <h2 className="text-lg font-semibold">Start Auction</h2>

        <div>
          <label className="block text-sm text-gray-600 mb-1">
            Starting Price
          </label>
          <p className="font-medium">${startingPrice.toFixed(2)}</p>
        </div>

        <div>
          <label className="block text-sm text-gray-600 mb-1">Start Time</label>
          <input
            type="datetime-local"
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
            className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label className="block text-sm text-gray-600 mb-1">End Time</label>
          <input
            type="datetime-local"
            value={endTime}
            onChange={(e) => setEndTime(e.target.value)}
            className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        {error && <p className="text-red-500 text-sm">{error}</p>}

        <div className="flex justify-end gap-3 pt-2">
          <button
            onClick={onClose}
            className="px-4 py-2 border rounded hover:bg-gray-50"
          >
            Cancel
          </button>
          <button
            onClick={() => createAuction.mutate()}
            disabled={!startTime || !endTime || createAuction.isPending}
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
          >
            {createAuction.isPending ? "Creating..." : "Create Auction"}
          </button>
        </div>
      </div>
    </div>
  );
}
