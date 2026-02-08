import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "../api";
import { listingsApi } from "@/features/listings/api";
import { auctionsApi } from "@/features/auctions/api";
import { PagedList } from "@/shared/components/PagedList";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { format } from "date-fns";
import { X } from "lucide-react";

type WatchlistTab = "Listings" | "Auctions";

export function WatchlistTab() {
  const [tab, setTab] = useState<WatchlistTab>("Listings");
  const [page, setPage] = useState(1);
  const queryClient = useQueryClient();

  const { data: listingWatchlist, isLoading: loadingListings } = useQuery({
    queryKey: ["watchlist", "listings", { page }],
    queryFn: () =>
      profileApi.getListingWatchlist({ pageNumber: page, pageSize: 10 }),
    enabled: tab === "Listings",
  });

  const { data: auctionWatchlist, isLoading: loadingAuctions } = useQuery({
    queryKey: ["watchlist", "auctions", { page }],
    queryFn: () =>
      profileApi.getAuctionWatchlist({ pageNumber: page, pageSize: 10 }),
    enabled: tab === "Auctions",
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

  const isLoading = tab === "Listings" ? loadingListings : loadingAuctions;

  return (
    <div className="space-y-4">
      {/* Sub tabs */}
      <div className="flex gap-1 border-b">
        {(["Listings", "Auctions"] as WatchlistTab[]).map((t) => (
          <button
            key={t}
            onClick={() => {
              setTab(t);
              setPage(1);
            }}
            className={cn(
              "px-4 py-2 text-sm font-medium border-b-2 transition-colors",
              tab === t
                ? "border-orange-500 text-orange-600"
                : "border-transparent text-gray-500 hover:text-gray-800",
            )}
          >
            {t}
          </button>
        ))}
      </div>

      {isLoading ? (
        <LoadingSpinner />
      ) : tab === "Listings" ? (
        !listingWatchlist?.items.length ? (
          <EmptyState
            title="No watched listings"
            description="Add listings to your watchlist to track them here."
          />
        ) : (
          <>
            <div className="space-y-3">
              {listingWatchlist.items.map((item) => (
                <div
                  key={item.listingId}
                  className="flex items-center justify-between bg-white border rounded-lg px-5 py-4"
                >
                  <Link
                    to={`/listings/${item.listingId}`}
                    className="flex-1 min-w-0"
                  >
                    <p className="font-medium text-gray-900 truncate">
                      {item.title}
                    </p>
                    <p className="text-sm text-gray-500 mt-0.5">
                      {item.listingType === "FixedPrice"
                        ? `$${item.buyNowPrice?.toFixed(2)}`
                        : "Auction"}{" "}
                      · Watched{" "}
                      {format(new Date(item.watchedSince), "dd MMM yyyy")}
                    </p>
                  </Link>
                  <div className="flex items-center gap-3 shrink-0 ml-4">
                    <StatusBadge status={item.status} />
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-8 w-8 text-gray-400 hover:text-red-500"
                      onClick={() => removeListingWatch.mutate(item.listingId)}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
            <PagedList
              currentPage={listingWatchlist.pageNumber}
              totalPages={listingWatchlist.totalPages}
              onPageChange={setPage}
            />
          </>
        )
      ) : !auctionWatchlist?.items.length ? (
        <EmptyState
          title="No watched auctions"
          description="Add auctions to your watchlist to track them here."
        />
      ) : (
        <>
          <div className="space-y-3">
            {auctionWatchlist.items.map((a) => (
              <div
                key={a.id}
                className="flex items-center justify-between bg-white border rounded-lg px-5 py-4"
              >
                <Link to={`/auctions/${a.id}`} className="flex-1 min-w-0">
                  <p className="font-medium text-gray-900 truncate">
                    {a.listingTitle}
                  </p>
                  <p className="text-sm text-gray-500 mt-0.5">
                    {a.currentHighestBid != null
                      ? `$${a.currentHighestBid.toFixed(2)}`
                      : `$${a.startingPrice.toFixed(2)}`}{" "}
                    · Ends {format(new Date(a.endTime), "dd MMM yyyy HH:mm")}
                  </p>
                </Link>
                <div className="flex items-center gap-3 shrink-0 ml-4">
                  <StatusBadge status={a.status} />
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-gray-400 hover:text-red-500"
                    onClick={() => removeAuctionWatch.mutate(a.id)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            ))}
          </div>
          <PagedList
            currentPage={auctionWatchlist.pageNumber}
            totalPages={auctionWatchlist.totalPages}
            onPageChange={setPage}
          />
        </>
      )}
    </div>
  );
}
