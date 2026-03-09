import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { profileApi } from "../api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { PagedList } from "@/shared/components/PagedList";
import { cn } from "@/lib/utils";
import { format, formatDistanceToNow } from "date-fns";
import { BuyingCard } from "@/shared/components/BuyingCard";

type BuyingFilter = "Active" | "Won" | "Lost" | "Purchased";

const FILTERS: { label: string; value: BuyingFilter }[] = [
  { label: "Bidding", value: "Active" },
  { label: "Won", value: "Won" },
  { label: "Lost", value: "Lost" },
  { label: "Purchased", value: "Purchased" },
];

export function BuyingTab() {
  const [filter, setFilter] = useState<BuyingFilter>("Active");
  const [page, setPage] = useState(1);

  const { data: auctionData, isLoading: loadingAuctions } = useQuery({
    queryKey: ["auctions", "bidding", { filter, page }],
    queryFn: () =>
      profileApi.getMyBuying({
        status: filter as "Active" | "Won" | "Lost",
        pageNumber: page,
        pageSize: 10,
      }),
    enabled: filter !== "Purchased",
  });

  const { data: purchaseData, isLoading: loadingPurchases } = useQuery({
    queryKey: ["payments", "purchases", { page }],
    queryFn: () =>
      profileApi.getFixedPricePurchases({ pageNumber: page, pageSize: 10 }),
    enabled: filter === "Purchased",
  });

  const isLoading = filter === "Purchased" ? loadingPurchases : loadingAuctions;

  return (
    <div className="space-y-4">
      <div className="flex gap-1 border-b">
        {FILTERS.map((f) => (
          <button
            key={f.value}
            onClick={() => {
              setFilter(f.value);
              setPage(1);
            }}
            className={cn(
              "px-4 py-2 text-sm font-medium border-b-2 transition-colors",
              filter === f.value
                ? "border-orange-500 text-orange-600"
                : "border-transparent text-gray-500 hover:text-gray-800",
            )}
          >
            {f.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <LoadingSpinner />
      ) : filter !== "Purchased" ? (
        !auctionData?.items.length ? (
          <EmptyState
            title={`No ${filter.toLowerCase()} auctions`}
            description="Nothing to show here."
          />
        ) : (
          <>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {auctionData.items.map((a) => (
                <BuyingCard
                  key={a.id}
                  listingId={a.listingId}
                  title={a.listingTitle}
                  thumbnailUrl={a.thumbnailUrl}
                  badge="Auction"
                  badgeColor="orange"
                  price={`$${(a.currentHighestBid ?? a.startingPrice).toFixed(2)}`}
                  sublabel={
                    filter === "Active"
                      ? `Ends ${formatDistanceToNow(new Date(a.endTime), { addSuffix: true })}`
                      : filter === "Lost"
                        ? `Ended ${format(new Date(a.closedAt ?? a.endTime), "dd MMM yyyy")}`
                        : `Won ${format(new Date(a.closedAt ?? a.endTime), "dd MMM yyyy")}`
                  }
                  statusLabel={
                    filter === "Active" && a.currentHighestBidderId
                      ? "Outbid"
                      : filter === "Won"
                        ? "Awaiting payment"
                        : undefined
                  }
                  statusColor={filter === "Won" ? "orange" : "orange"}
                />
              ))}
            </div>
            <PagedList
              currentPage={auctionData.pageNumber}
              totalPages={auctionData.totalPages}
              onPageChange={setPage}
            />
          </>
        )
      ) : !purchaseData?.items.length ? (
        <EmptyState
          title="No purchases"
          description="You have not bought any fixed price listings yet."
        />
      ) : (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {purchaseData.items.map((p) => (
              <BuyingCard
                key={p.id}
                listingId={p.listingId}
                title={
                  p.listingTitle ?? `Listing ${p.listingId.slice(0, 8)}...`
                }
                thumbnailUrl={p.thumbnailUrl}
                badge="Fixed Price"
                badgeColor="blue"
                price={`$${p.amount.toFixed(2)}`}
                sublabel={`Purchased ${format(new Date(p.paidAt ?? p.createdAt), "dd MMM yyyy")}`}
              />
            ))}
          </div>
          <PagedList
            currentPage={purchaseData.pageNumber}
            totalPages={purchaseData.totalPages}
            onPageChange={setPage}
          />
        </>
      )}
    </div>
  );
}
