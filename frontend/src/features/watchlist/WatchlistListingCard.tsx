import { StatusBadge } from "@/shared/components/StatusBadge";
import type { WatchlistItemDto } from "@/shared/types/common";
import { formatDistanceToNow, intlFormatDistance } from "date-fns";
import { X } from "lucide-react";
import { Link } from "react-router-dom";

export function WatchlistListingCard({
  item,
  onRemove,
}: {
  item: WatchlistItemDto;
  onRemove: () => void;
}) {
  return (
    <div className="relative group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm hover:shadow-md transition-shadow">
      <Link to={`/listings/${item.listingId}`} className="flex flex-col flex-1">
        <div className="relative h-48 overflow-hidden bg-gray-100">
          {item.thumbnailUrl ? (
            <img
              src={item.thumbnailUrl}
              alt={item.title}
              className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
            />
          ) : (
            <div className="flex h-full items-center justify-center text-gray-300">
              No image
            </div>
          )}
          <div className="absolute top-2 right-2">
            <span className="rounded-full bg-white/90 px-2 py-0.5 text-xs font-medium text-gray-600 shadow-sm">
              {item.listingType === "Auction" ? "Auction" : "Buy Now"}
            </span>
          </div>
        </div>
        <div className="flex flex-1 flex-col gap-2 p-4">
          <div className="flex items-start justify-between gap-2">
            <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors flex-1">
              {item.title}
            </h3>
            <StatusBadge status={item.status} />
          </div>
          <div className="mt-auto space-y-1">
            {item.listingType === "FixedPrice" && item.buyNowPrice != null ? (
              <p className="text-lg font-bold text-gray-900">
                ${item.buyNowPrice.toLocaleString()}
              </p>
            ) : (
              <p className="text-sm font-medium text-gray-500">
                See auction for bid
              </p>
            )}
          </div>
          <p className="text-xs text-gray-400">
            Added {intlFormatDistance(new Date(item.watchedSince), new Date())}
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
