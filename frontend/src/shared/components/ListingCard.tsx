import { Link } from "react-router-dom";
import { MapPin, Tag, Heart } from "lucide-react";
import type { ListingDto } from "@/shared/types/common";
import { formatDistanceToNow } from "date-fns";
import { useState } from "react";
import { listingsApi } from "@/features/listings/api";
import { useAuth } from "@/features/auth/AuthContext";
import { useQueryClient } from "@tanstack/react-query";

interface ListingCardProps {
  listing: ListingDto;
  isWatched?: boolean;
}

export function ListingCard({
  listing,
  isWatched: initialWatched = false,
}: ListingCardProps) {
  const thumbnail = listing.images[0]?.url;
  const { isAuthenticated, user } = useAuth();
  const watched = initialWatched;
  const [loading, setLoading] = useState(false);

  const queryClient = useQueryClient();
  const handleWatch = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (loading || !isAuthenticated) return;
    setLoading(true);

    // Optimistic update
    queryClient.setQueryData<{ items: { listingId: string }[] }>(
      ["listings-watchlist"],
      (prev) => {
        if (!prev) return prev;
        return {
          ...prev,
          items: watched
            ? prev.items.filter((w) => w.listingId !== listing.id)
            : ([...prev.items, { listingId: listing.id }] as typeof prev.items),
        };
      },
    );

    try {
      if (watched) {
        await listingsApi.removeFromWatchlist(listing.id);
      } else {
        await listingsApi.addToWatchlist(listing.id);
      }
    } catch (err) {
      // Revert optimistic update on failure
      queryClient.invalidateQueries({ queryKey: ["listings-watchlist"] });
      console.error("Failed to update watchlist", err);
    } finally {
      setLoading(false);
    }
  };
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
        <div className="absolute top-2 right-2">
          <span className="rounded-full bg-white/90 px-2 py-0.5 text-xs font-medium text-gray-600 shadow-sm">
            {listing.listingType === "Auction" ? "Auction" : "Buy Now"}
          </span>
        </div>
        {isAuthenticated && user?.id !== listing.sellerId && (
          <button
            onClick={handleWatch}
            disabled={loading}
            className="absolute bottom-2 right-2 flex h-8 w-8 items-center justify-center rounded-full bg-white/90 shadow-sm transition-colors hover:bg-white"
          >
            <Heart
              className={`h-4 w-4 transition-colors ${
                watched ? "fill-orange-500 text-orange-500" : "text-gray-400"
              }`}
            />
          </button>
        )}
      </div>

      <div className="flex flex-1 flex-col gap-2 p-4">
        <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors">
          {listing.title}
        </h3>

        <div className="mt-auto space-y-1">
          {listing.listingType === "FixedPrice" &&
          listing.buyNowPrice !== undefined ? (
            <p className="text-lg font-bold text-gray-900">
              ${listing.buyNowPrice.toLocaleString()}
            </p>
          ) : (
            <p className="text-sm font-medium text-gray-500">
              See auction for bid
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
