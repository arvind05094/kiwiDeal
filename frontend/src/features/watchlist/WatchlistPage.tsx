import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { profileApi } from "@/features/profile/api";
import { listingsApi } from "@/features/listings/api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { Heart } from "lucide-react";
import type { WatchlistItemDto } from "@/shared/types/common";
import { WatchlistListingCard } from "./WatchlistListingCard";

export function WatchlistPage() {
  const queryClient = useQueryClient();

  const { data: listingData, isLoading } = useQuery({
    queryKey: ["watchlist", "listings", { pageNumber: 1 }],
    queryFn: () =>
      profileApi.getListingWatchlist({ pageNumber: 1, pageSize: 12 }),
    staleTime: 30_000,
  });
  const removeListingWatch = useMutation({
    mutationFn: (id: string) => listingsApi.removeFromWatchlist(id),
    onSuccess: (_, listingId) => {
      queryClient.invalidateQueries({ queryKey: ["watchlist", "listings"] });
      queryClient.invalidateQueries({
        queryKey: ["listing-watched", listingId],
      });
      queryClient.invalidateQueries({ queryKey: ["listings-watchlist"] });
    },
  });

  const listings: WatchlistItemDto[] = listingData?.items ?? [];

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="max-w-6xl mx-auto space-y-8">
      <div className="flex items-center gap-3">
        <Heart className="h-5 w-5 text-orange-500 fill-orange-500" />
        <h1 className="text-2xl font-semibold text-gray-900">Watchlist</h1>
      </div>

      {listings.length === 0 ? (
        <EmptyState
          title="Nothing on your watchlist"
          description="Watch listings and auctions to track them here."
        />
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {listings.map((item) => (
            <WatchlistListingCard
              key={item.listingId}
              item={item}
              onRemove={() => removeListingWatch.mutate(item.listingId)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
