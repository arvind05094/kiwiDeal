import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { profileApi } from "@/features/profile/api";
import { listingsApi } from "@/features/listings/api";
import { auctionsApi } from "@/features/auctions/api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { Heart } from "lucide-react";
import type { WatchlistItemDto, AuctionDto } from "@/shared/types/common";
import { WatchlistListingCard } from "./WatchlistListingCard";
import { WatchlistAuctionCard } from "./WatchlistAuctionCard";

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
