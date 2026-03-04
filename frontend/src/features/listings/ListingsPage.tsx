import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "./api";
import { ListingCard } from "@/shared/components/ListingCard";
import { CategoryTiles } from "@/shared/components/CategoryTiles";
import { PagedList } from "@/shared/components/PagedList";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { NZ_REGIONS } from "@/shared/types/common";
import { Search, X } from "lucide-react";

export function ListingsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [searchInput, setSearchInput] = useState(
    searchParams.get("searchTerm") ?? "",
  );

  const pageNumber = Number(searchParams.get("pageNumber") ?? 1);
  const searchTerm = searchParams.get("searchTerm") ?? undefined;
  const category = searchParams.get("category") ?? undefined;
  const region = searchParams.get("region") ?? undefined;
  const sortBy = (searchParams.get("sortBy") as any) ?? undefined;
  const listingType = (searchParams.get("listingType") as any) ?? undefined;

  const { data, isLoading, isError } = useQuery({
    queryKey: [
      "listings",
      { pageNumber, searchTerm, category, region, sortBy, listingType },
    ],
    queryFn: () =>
      listingsApi.getListings({
        pageNumber,
        pageSize: 12,
        searchTerm,
        category,
        region,
        sortBy,
        listingType,
      }),
  });

  const setParam = (key: string, value: string | undefined) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);
      if (value) next.set(key, value);
      else next.delete(key);
      next.delete("pageNumber");
      return next;
    });
  };

  const handleSearch = () => {
    setParam("searchTerm", searchInput || undefined);
  };

  const handleCategoryClick = (cat: string) => {
    setParam("category", category === cat ? undefined : cat);
  };

  const activeFilters = [
    category && { key: "category", label: category },
    region && { key: "region", label: region },
    listingType && {
      key: "listingType",
      label: listingType === "FixedPrice" ? "Fixed Price" : "Auction",
    },
    sortBy && { key: "sortBy", label: sortBy.replace("_", " ") },
  ].filter(Boolean) as { key: string; label: string }[];

  return (
    <div className="space-y-6">
      {/* Search bar */}
      <div className="flex gap-2">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 h-4 w-4" />
          <Input
            className="pl-9"
            placeholder="Search listings..."
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSearch()}
          />
        </div>
        <Button onClick={handleSearch}>Search</Button>
      </div>

      {/* Category tiles */}
      <CategoryTiles selected={category} onSelect={handleCategoryClick} />

      {/* Filters row */}
      <div className="flex flex-wrap gap-3 items-center">
        <Select
          value={listingType ?? "all"}
          onValueChange={(v) =>
            setParam("listingType", v === "all" ? undefined : v)
          }
        >
          <SelectTrigger className="w-36">
            <SelectValue placeholder="Type" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Types</SelectItem>
            <SelectItem value="Auction">Auction</SelectItem>
            <SelectItem value="FixedPrice">Fixed Price</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={region ?? "all"}
          onValueChange={(v) => setParam("region", v === "all" ? undefined : v)}
        >
          <SelectTrigger className="w-44">
            <SelectValue placeholder="Region" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Regions</SelectItem>
            {NZ_REGIONS.map((r) => (
              <SelectItem key={r.value} value={r.value}>
                {r.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        <Select
          value={sortBy ?? "newest"}
          onValueChange={(v) =>
            setParam("sortBy", v === "newest" ? undefined : v)
          }
        >
          <SelectTrigger className="w-44">
            <SelectValue placeholder="Sort by" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="newest">Newest</SelectItem>
            <SelectItem value="price_asc">Price: Low to High</SelectItem>
            <SelectItem value="price_desc">Price: High to Low</SelectItem>
            <SelectItem value="ending_soon">Ending Soon</SelectItem>
          </SelectContent>
        </Select>

        {/* Active filter pills */}
        {activeFilters.map((f) => (
          <span
            key={f.key}
            className="flex items-center gap-1 bg-gray-100 text-gray-700 text-sm px-3 py-1 rounded-full"
          >
            {f.label}
            <button
              onClick={() => setParam(f.key, undefined)}
              className="hover:text-gray-900"
            >
              <X className="h-3 w-3" />
            </button>
          </span>
        ))}
      </div>

      {/* Results */}
      {isLoading ? (
        <LoadingSpinner />
      ) : isError ? (
        <EmptyState
          title="Unable to load listings"
          message="The server could not be reached. Please check your connection and try again."
        />
      ) : !data || data.items.length === 0 ? (
        <EmptyState
          title="No listings found"
          message="Try adjusting your filters or search term."
        />
      ) : (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {data.items.map((listing) => (
              <ListingCard key={listing.id} listing={listing} />
            ))}
          </div>
          <PagedList
            currentPage={data.pageNumber}
            totalPages={data.totalPages}
            hasPreviousPage={data.hasPreviousPage}
            hasNextPage={data.hasNextPage}
            onPageChange={(p) =>
              setSearchParams((prev) => {
                const next = new URLSearchParams(prev);
                next.set("pageNumber", String(p));
                return next;
              })
            }
          />
        </>
      )}
    </div>
  );
}
