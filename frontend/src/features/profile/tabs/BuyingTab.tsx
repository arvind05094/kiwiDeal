import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "../api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
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
                <Link
                  key={a.id}
                  to={`/listings/${a.listingId}`}
                  className="group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm transition-shadow hover:shadow-md"
                >
                  <div className="h-48 overflow-hidden bg-gray-100">
                    {a.thumbnailUrl ? (
                      <img
                        src={a.thumbnailUrl}
                        alt={a.listingTitle}
                        className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
                      />
                    ) : (
                      <div className="flex h-full items-center justify-center text-gray-300 text-sm">
                        No image
                      </div>
                    )}
                  </div>
                  <div className="flex flex-1 flex-col gap-2 p-4">
                    <span className="self-start rounded-full px-2 py-0.5 text-xs font-medium bg-orange-100 text-orange-600">
                      Auction
                    </span>
                    <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors">
                      {a.listingTitle}
                    </h3>
                    <div className="mt-auto space-y-1">
                      <p className="text-lg font-bold text-gray-900">
                        ${(a.currentHighestBid ?? a.startingPrice).toFixed(2)}
                      </p>
                      <p className="text-xs text-gray-500">
                        {filter === "Active"
                          ? `Ends ${formatDistanceToNow(new Date(a.endTime), { addSuffix: true })}`
                          : filter === "Lost"
                            ? `Auction ended on ${format(new Date(a.closedAt ?? a.endTime), "dd MMM yyyy")}`
                            : `You won this auction on ${format(new Date(a.closedAt ?? a.endTime), "dd MMM yyyy")}`}
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
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {purchaseData.items.map((p) => (
              <Link
                key={p.id}
                to={`/listings/${p.listingId}`}
                className="group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm transition-shadow hover:shadow-md"
              >
                <div className="h-48 overflow-hidden bg-gray-100">
                  {p.thumbnailUrl ? (
                    <img
                      src={p.thumbnailUrl}
                      alt={p.listingTitle ?? "Purchase"}
                      className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
                    />
                  ) : (
                    <div className="flex h-full items-center justify-center text-gray-300 text-sm">
                      No image
                    </div>
                  )}
                </div>
                <div className="flex flex-1 flex-col gap-2 p-4">
                  <span className="self-start rounded-full px-2 py-0.5 text-xs font-medium bg-blue-100 text-blue-600">
                    Fixed Price
                  </span>
                  <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors">
                    {p.listingTitle ?? `Listing ${p.listingId.slice(0, 8)}...`}
                  </h3>
                  <div className="mt-auto space-y-1">
                    <p className="text-lg font-bold text-gray-900">
                      ${p.amount.toFixed(2)}
                    </p>
                    <p className="text-xs text-gray-500">
                      You purchased this on{" "}
                      {format(new Date(p.paidAt ?? p.createdAt), "dd MMM yyyy")}
                    </p>
                  </div>
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
