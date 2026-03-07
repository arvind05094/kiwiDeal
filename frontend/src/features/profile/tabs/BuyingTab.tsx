import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "../api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { PagedList } from "@/shared/components/PagedList";
import { cn } from "@/lib/utils";
import { format, formatDistanceToNow } from "date-fns";

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
        status: filter,
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
            <div className="space-y-3">
              {auctionData.items.map((a) => (
                <Link
                  key={a.id}
                  to={`/listings/${a.listingId}`}
                  className="flex items-center justify-between bg-white border rounded-lg px-5 py-4 hover:shadow-sm transition-shadow"
                >
                  <div>
                    <p className="font-medium text-gray-900">
                      {a.listingTitle}
                    </p>
                    <p className="text-sm text-gray-500 mt-0.5">
                      {filter === "Active"
                        ? `Ends ${formatDistanceToNow(new Date(a.endTime), { addSuffix: true })}`
                        : filter === "Lost"
                          ? `Auction ended on ${format(new Date(a.closedAt ?? a.endTime), "dd MMM yyyy")}`
                          : `You won this auction on ${format(new Date(a.closedAt ?? a.endTime), "dd MMM yyyy")}`}
                    </p>
                  </div>
                  <div className="flex flex-col items-end gap-0.5 shrink-0">
                    <p className="text-sm font-medium text-gray-900">
                      ${(a.currentHighestBid ?? a.startingPrice).toFixed(2)}
                    </p>
                    {filter === "Active" && a.currentHighestBidderId && (
                      <p className="text-xs text-orange-500 font-medium">
                        Outbid
                      </p>
                    )}
                    {filter === "Won" && (
                      <p className="text-xs text-orange-500 font-medium">
                        Awaiting payment
                      </p>
                    )}
                  </div>
                </Link>
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
          <div className="space-y-3">
            {purchaseData.items.map((p) => (
              <Link
                key={p.id}
                to={`/listings/${p.listingId}`}
                className="flex items-center justify-between bg-white border rounded-lg px-5 py-4 hover:shadow-sm transition-shadow"
              >
                <div>
                  <p className="font-medium text-gray-900">
                    Listing {p.listingId.slice(0, 8)}...
                  </p>
                  <p className="text-sm text-gray-500 mt-0.5">
                    You purchased this on{" "}
                    {format(new Date(p.paidAt ?? p.createdAt), "dd MMM yyyy")}
                  </p>
                </div>
                <div className="flex items-center gap-4 shrink-0">
                  <p className="text-sm font-medium text-gray-900">
                    ${p.amount.toFixed(2)}
                  </p>
                  <StatusBadge status={p.status} />
                </div>
              </Link>
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
