import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "@/features/profile/api";
import { listingsApi } from "@/features/listings/api";
import { auctionsApi } from "@/features/auctions/api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { Heart, X, Gavel } from "lucide-react";
import { formatDistanceToNow, format } from "date-fns";
import type { WatchlistItemDto, AuctionDto } from "@/shared/types/common";

// --- Local card components ---

function WatchlistListingCard({
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
          <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors">
            {item.title}
          </h3>
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
            <div className="flex items-center justify-between">
              <StatusBadge status={item.status} />
              <p className="text-xs text-gray-400">
                {formatDistanceToNow(new Date(item.watchedSince), {
                  addSuffix: true,
                })}
              </p>
            </div>
          </div>
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

function WatchlistAuctionCard({
  auction,
  onRemove,
}: {
  auction: AuctionDto;
  onRemove: () => void;
}) {
  return (
    <div className="relative group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm hover:shadow-md transition-shadow">
      <Link to={`/auctions/${auction.id}`} className="flex flex-col flex-1 p-4 gap-3">
        <div className="flex items-start justify-between gap-2">
          <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors flex-1">
            {auction.listingTitle}
          </h3>
          <Gavel className="h-4 w-4 text-gray-300 shrink-0 mt-0.5" />
        </div>
        <div className="mt-auto space-y-2">
          <div>
            <p className="text-xs text-gray-400">Current bid</p>
            <p className="text-lg font-bold text-gray-900">
              ${(auction.currentHighestBid ?? auction.startingPrice).toLocaleString()}
            </p>
          </div>
          <div className="flex items-center justify-between">
            <StatusBadge status={auction.status} />
            <p className="text-xs text-gray-400">
              Ends {format(new Date(auction.endTime), "dd MMM HH:mm")}
            </p>
          </div>
          <p className="text-xs text-gray-400">
            {auction.bids.length} bid{auction.bids.length !== 1 ? "s" : ""}
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

// --- Page ---

export function WatchlistPage() {
  const [listingPage, setListingPage] = useState(1);
  const [auctionPage, setAuctionPage] = useState(1);
  const queryClient = useQueryClient();

  const { data: listingData, isLoading: loadingListings } = useQuery({
    queryKey: ["watchlist", "listings", { listingPage }],
    queryFn: () =>
      profileApi.getListingWatchlist({ pageNumber: listingPage, pageSize: 12 }),
    staleTime: 30_000,
  });

  const { data: auctionData, isLoading: loadingAuctions } = useQuery({
    queryKey: ["watchlist", "auctions", { auctionPage }],
    queryFn: () =>
      profileApi.getAuctionWatchlist({ pageNumber: auctionPage, pageSize: 12 }),
    staleTime: 30_000,
  });

  const removeListingWatch = useMutation({
    mutationFn: (id: string) => listingsApi.removeFromWatchlist(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["watchlist", "listings"] });
      queryClient.invalidateQueries({ queryKey: ["listings-watchlist"] });
    },
  });

  const removeAuctionWatch = useMutation({
    mutationFn: (id: string) => auctionsApi.removeFromWatchlist(id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["watchlist", "auctions"] }),
  });

  const listings: WatchlistItemDto[] = listingData?.items ?? [];
  const auctions: AuctionDto[] = auctionData?.items ?? [];
  const isLoading = loadingListings || loadingAuctions;
  const hasItems = listings.length > 0 || auctions.length > 0;

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="max-w-6xl mx-auto space-y-8">
      <div className="flex items-center gap-3">
        <Heart className="h-5 w-5 text-orange-500 fill-orange-500" />
        <h1 className="text-2xl font-semibold text-gray-900">Watchlist</h1>
      </div>

      {!hasItems ? (
        <EmptyState
          title="Nothing on your watchlist"
          description="Watch listings and auctions to track them here."
        />
      ) : (
        <div className="space-y-10">
          {listings.length > 0 && (
            <section>
              <h2 className="text-base font-semibold text-gray-700 mb-4">
                Listings
              </h2>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                {listings.map((item) => (
                  <WatchlistListingCard
                    key={item.listingId}
                    item={item}
                    onRemove={() => removeListingWatch.mutate(item.listingId)}
                  />
                ))}
              </div>
              {listingData && listingPage < listingData.totalPages && (
                <div className="mt-4 flex justify-center">
                  <button
                    onClick={() => setListingPage((p) => p + 1)}
                    className="text-sm text-gray-500 hover:text-gray-900 underline"
                  >
                    Load more listings
                  </button>
                </div>
              )}
            </section>
          )}

          {auctions.length > 0 && (
            <section>
              <h2 className="text-base font-semibold text-gray-700 mb-4">
                Auctions
              </h2>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                {auctions.map((auction) => (
                  <WatchlistAuctionCard
                    key={auction.id}
                    auction={auction}
                    onRemove={() => removeAuctionWatch.mutate(auction.id)}
                  />
                ))}
              </div>
              {auctionData && auctionPage < auctionData.totalPages && (
                <div className="mt-4 flex justify-center">
                  <button
                    onClick={() => setAuctionPage((p) => p + 1)}
                    className="text-sm text-gray-500 hover:text-gray-900 underline"
                  >
                    Load more auctions
                  </button>
                </div>
              )}
            </section>
          )}
        </div>
      )}
    </div>
  );
}
