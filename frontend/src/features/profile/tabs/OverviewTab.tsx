import { useQuery } from "@tanstack/react-query";
import { profileApi } from "../api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { LayoutList, Gavel, ShoppingBag } from "lucide-react";

export function OverviewTab() {
  const { data: listings, isLoading: loadingListings } = useQuery({
    queryKey: ["listings", "mine", { status: "Active" }],
    queryFn: () =>
      profileApi.getMyListings({
        statuses: ["Active"],
        pageNumber: 1,
        pageSize: 1,
      }),
  });

  const { data: selling, isLoading: loadingSelling } = useQuery({
    queryKey: ["auctions", "mine", { status: "Active" }],
    queryFn: () =>
      profileApi.getMySelling({ status: "Active", pageNumber: 1, pageSize: 1 }),
  });

  const { data: sales, isLoading: loadingSales } = useQuery({
    queryKey: ["payments", "sales", { pageNumber: 1 }],
    queryFn: () =>
      profileApi.getFixedPriceSales({ pageNumber: 1, pageSize: 1 }),
  });

  if (loadingListings || loadingSelling || loadingSales)
    return <LoadingSpinner />;

  const stats = [
    {
      label: "Active Listings",
      value: listings?.totalCount ?? 0,
      icon: LayoutList,
      color: "text-blue-600 bg-blue-50",
    },
    {
      label: "Active Auctions",
      value: selling?.totalCount ?? 0,
      icon: Gavel,
      color: "text-orange-600 bg-orange-50",
    },
    {
      label: "Total Sales",
      value: sales?.totalCount ?? 0,
      icon: ShoppingBag,
      color: "text-green-600 bg-green-50",
    },
  ];

  return (
    <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
      {stats.map((s) => (
        <div
          key={s.label}
          className="bg-white border rounded-lg p-6 flex items-center gap-4"
        >
          <div className={`p-3 rounded-lg ${s.color}`}>
            <s.icon className="h-6 w-6" />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900">{s.value}</p>
            <p className="text-sm text-gray-500">{s.label}</p>
          </div>
        </div>
      ))}
    </div>
  );
}
