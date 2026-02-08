import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "../api";
import { PagedList } from "@/shared/components/PagedList";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { format } from "date-fns";

export function FixedPricePurchasesTab() {
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ["payments", "purchases", { page }],
    queryFn: () =>
      profileApi.getFixedPricePurchases({ pageNumber: page, pageSize: 10 }),
  });

  if (isLoading) return <LoadingSpinner />;
  if (!data?.items.length)
    return (
      <EmptyState
        title="No fixed price purchases"
        description="You have not bought any items via Buy Now yet."
      />
    );

  return (
    <div className="space-y-4">
      <div className="space-y-3">
        {data.items.map((l) => (
          <Link
            key={l.id}
            to={`/listings/${l.id}`}
            className="flex items-center justify-between bg-white border rounded-lg px-5 py-4 hover:shadow-sm transition-shadow"
          >
            <div>
              <p className="font-medium text-gray-900">{l.title}</p>
              <p className="text-sm text-gray-500 mt-0.5">
                {l.category} · {l.region}
              </p>
            </div>
            <div className="text-right shrink-0">
              <p className="font-semibold text-gray-900">
                ${l.buyNowPrice?.toFixed(2) ?? "—"}
              </p>
              <p className="text-xs text-gray-400">
                {format(new Date(l.updatedAt), "dd MMM yyyy")}
              </p>
            </div>
          </Link>
        ))}
      </div>
      <PagedList
        currentPage={data.pageNumber}
        totalPages={data.totalPages}
        onPageChange={setPage}
      />
    </div>
  );
}
