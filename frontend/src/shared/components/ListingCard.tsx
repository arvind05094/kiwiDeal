import { Link } from "react-router-dom";
import { MapPin, Tag } from "lucide-react";
import { StatusBadge } from "./StatusBadge";
import type { ListingDto } from "@/shared/types/common";
import { formatDistanceToNow } from "date-fns";

interface ListingCardProps {
  listing: ListingDto;
}

export function ListingCard({ listing }: ListingCardProps) {
  const thumbnail = listing.images[0]?.url;

  return (
    <Link
      to={`/listings/${listing.id}`}
      className="group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    >
      <div className="relative h-48 overflow-hidden bg-gray-100">
        {thumbnail ? (
          <img
            src={thumbnail}
            alt={listing.title}
            className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-gray-300">
            No image
          </div>
        )}
        <div className="absolute top-2 left-2">
          <StatusBadge status={listing.status} />
        </div>
        <div className="absolute top-2 right-2">
          <span className="rounded-full bg-white/90 px-2 py-0.5 text-xs font-medium text-gray-600 shadow-sm">
            {listing.listingType === "Auction" ? "Auction" : "Buy Now"}
          </span>
        </div>
      </div>

      <div className="flex flex-1 flex-col gap-2 p-4">
        <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors">
          {listing.title}
        </h3>

        <div className="mt-auto space-y-1">
          {listing.buyNowPrice !== undefined && (
            <p className="text-lg font-bold text-gray-900">
              ${listing.buyNowPrice.toLocaleString()}
            </p>
          )}

          <div className="flex items-center justify-between text-xs text-gray-500">
            <div className="flex items-center gap-1">
              <MapPin className="h-3 w-3" />
              {listing.region}
            </div>
            <div className="flex items-center gap-1">
              <Tag className="h-3 w-3" />
              {listing.category}
            </div>
          </div>

          <p className="text-xs text-gray-400">
            {formatDistanceToNow(new Date(listing.createdAt), {
              addSuffix: true,
            })}
          </p>
        </div>
      </div>
    </Link>
  );
}
