import { StatusBadge } from "@/shared/components/StatusBadge";
import type { AuctionDto } from "@/shared/types/common";
import { format } from "date-fns";
import { Gavel, X } from "lucide-react";
import { Link } from "react-router-dom";

export function WatchlistAuctionCard({
  auction,
  onRemove,
}: {
  auction: AuctionDto;
  onRemove: () => void;
}) {
  return (
    <div className="relative group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm hover:shadow-md transition-shadow">
      <Link
        to={`/auctions/${auction.id}`}
        className="flex flex-col flex-1 p-4 gap-3"
      >
        <div className="flex items-start justify-between gap-2">
          <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors flex-1">
            {auction.listingTitle}
          </h3>
          <div className="flex items-center gap-1.5 shrink-0">
            <StatusBadge status={auction.status} />
            <Gavel className="h-4 w-4 text-gray-300" />
          </div>
        </div>
        <div className="mt-auto space-y-2">
          <div>
            <p className="text-xs text-gray-400">Current bid</p>
            <p className="text-lg font-bold text-gray-900">
              $
              {(
                auction.currentHighestBid ?? auction.startingPrice
              ).toLocaleString()}
            </p>
          </div>
          <p className="text-xs text-gray-400">
            {auction.bids?.length ?? 0} bid
            {(auction.bids?.length ?? 0) !== 1 ? "s" : ""}
          </p>
        </div>
      </Link>
      <button
        onClick={onRemove}
        className="absolute top-2 left-2 flex h-7 w-7 items-center justify-center rounded-full bg-white/90 shadow-sm hover:bg-red-50 hover:text-red-500 text-gray-400 transition-colors"
      >
        <X className="h-3.5 w-3.5" />
      </button>
    </div>
  );
}
