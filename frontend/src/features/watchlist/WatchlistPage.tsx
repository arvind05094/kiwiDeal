import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "@/features/profile/api";
import { listingsApi } from "@/features/listings/api";
import { auctionsApi } from "@/features/auctions/api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { Button } from "@/components/ui/button";
import { format } from "date-fns";
import { Heart, X } from "lucide-react";
import type { WatchlistItemDto, AuctionDto } from "@/shared/types/common";

export function WatchlistPage() {
  const [listingPage, setListingPage] = useState(1);
  const [auctionPage, setAuctionPage] = useState(1);
  const queryClient = useQueryClient();

  const { data: listingData, isLoading: loadingListings } = useQuery({
    queryKey: ["watchlist", "listings", { listingPage }],
    queryFn: () =>
      profileApi.getListingWatchlist({ pageNumber: listingPage, pageSize: 10 }),
    staleTime: 30_000,
  });

  const { data: auctionData, isLoading: loadingAuctions } = useQuery({
    queryKey: ["watchlist", "auctions", { auctionPage }],
    queryFn: () =>
      profileApi.getAuctionWatchlist({ pageNumber: auctionPage, pageSize: 10 }),
    staleTime: 30_000,
  });

  const removeListingWatch = useMutation({
    mutationFn: (id: string) => listingsApi.removeFromWatchlist(id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["watchlist", "listings"] }),
  });

  const removeAuctionWatch = useMutation({
    mutationFn: (id: string) => auctionsApi.removeFromWatchlist(id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["watchlist", "auctions"] }),
  });

  const listings: WatchlistItemDto[] = listingData?.items ?? [];
  const auctions: AuctionDto[] = auctionData?.items ?? [];

  const combined = [
    ...listings.map((l) => ({
      type: "listing" as const,
      date: new Date(l.watchedSince),
      data: l,
    })),
    ...auctions.map((a) => ({
      type: "auction" as const,
      date: new Date(a.endTime),
      data: a,
    })),
  ].sort((a, b) => b.date.getTime() - a.date.getTime());

  const isLoading = loadingListings || loadingAuctions;
  const hasMoreListings = listingData
    ? listingPage < listingData.totalPages
    : false;
  const hasMoreAuctions = auctionData
    ? auctionPage < auctionData.totalPages
    : false;
  const hasMore = hasMoreListings || hasMoreAuctions;

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center gap-3">
        <Heart className="h-5 w-5 text-orange-500" />
        <h1 className="text-2xl font-semibold text-gray-900">Watchlist</h1>
      </div>

      {combined.length === 0 ? (
        <EmptyState
          title="Nothing on watchlist"
          message="Watch listings and auctions to track them here."
        />
      ) : (
        <div className="space-y-3">
          {combined.map((item) =>
            item.type === "listing" ? (
              <div
                key={`l-${item.data.listingId}`}
                className="flex items-center justify-between bg-white border rounded-lg px-5 py-4"
              >
                <Link
                  to={`/listings/${item.data.listingId}`}
                  className="flex-1 min-w-0"
                >
                  <p className="font-medium text-gray-900 truncate">
                    {item.data.title}
                  </p>
                  <p className="text-sm text-gray-500 mt-0.5">
                    {item.data.listingType === "FixedPrice"
                      ? `$${item.data.buyNowPrice?.toFixed(2)}`
                      : "Auction"}{" "}
                    · Watched {format(item.date, "dd MMM yyyy")}
                  </p>
                </Link>
                <div className="flex items-center gap-3 shrink-0 ml-4">
                  <StatusBadge status={item.data.status} />
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-gray-400 hover:text-red-500"
                    onClick={() =>
                      removeListingWatch.mutate(item.data.listingId)
                    }
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            ) : (
              <div
                key={`a-${item.data.id}`}
                className="flex items-center justify-between bg-white border rounded-lg px-5 py-4"
              >
                <Link
                  to={`/auctions/${item.data.id}`}
                  className="flex-1 min-w-0"
                >
                  <p className="font-medium text-gray-900 truncate">
                    {item.data.listingTitle}
                  </p>
                  <p className="text-sm text-gray-500 mt-0.5">
                    {item.data.currentHighestBid != null
                      ? `$${item.data.currentHighestBid.toFixed(2)}`
                      : `$${item.data.startingPrice.toFixed(2)}`}{" "}
                    · Ends{" "}
                    {format(new Date(item.data.endTime), "dd MMM yyyy HH:mm")}
                  </p>
                </Link>
                <div className="flex items-center gap-3 shrink-0 ml-4">
                  <StatusBadge status={item.data.status} />
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-gray-400 hover:text-red-500"
                    onClick={() => removeAuctionWatch.mutate(item.data.id)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            ),
          )}

          {hasMore && (
            <div className="pt-2 flex justify-center">
              <Button
                variant="outline"
                onClick={() => {
                  if (hasMoreListings) setListingPage((p) => p + 1);
                  if (hasMoreAuctions) setAuctionPage((p) => p + 1);
                }}
              >
                Load more
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
