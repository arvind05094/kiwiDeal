import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "./api";
import Navbar from "@/shared/components/Navbar";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ListingDto } from "./types";

export default function MyListingsPage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const { data, isLoading, isError } = useQuery({
    queryKey: ["my-listings", page],
    queryFn: () => listingsApi.getMyListings(page, 10),
  });

  const listings = data?.data.items ?? [];
  const totalPages = data
    ? Math.ceil(data.data.totalCount / data.data.pageSize)
    : 1;

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-5xl mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-bold text-gray-800">My Listings</h1>
          <Button
            className="bg-orange-500 hover:bg-orange-600 text-white"
            onClick={() => navigate("/listings/new")}
          >
            + Post Listing
          </Button>
        </div>

        {isLoading && (
          <p className="text-gray-500 text-center py-4">Loading...</p>
        )}
        {isError && (
          <p className="text-red-500 text-center py-4">
            Failed to load listings.
          </p>
        )}
        {!isLoading && listings.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-500 mb-4">
              You haven't posted any listings yet.
            </p>
            <Button
              className="bg-orange-500 hover:bg-orange-600 text-white"
              onClick={() => navigate("/listings/new")}
            >
              Post your first listing
            </Button>
          </div>
        )}

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {listings.map((listing: ListingDto) => (
            <Card
              key={listing.id}
              className="cursor-pointer hover:shadow-md transition-shadow bg-white flex flex-col"
              onClick={() => navigate(`/listings/${listing.id}`)}
            >
              <div className="h-40 bg-gray-200 rounded-t-lg flex items-center justify-center overflow-hidden">
                {listing.images && listing.images.length > 0 ? (
                  <img
                    src={listing.images[0].url}
                    alt={listing.title}
                    className="object-cover w-full h-full"
                  />
                ) : (
                  <span className="text-gray-400 text-sm">No image</span>
                )}
              </div>
              <CardContent className="p-4 flex flex-col gap-2">
                <h3
                  className="font-semibold text-gray-800 truncate"
                  title={listing.title}
                >
                  {listing.title}
                </h3>
                <p className="text-orange-500 font-bold">
                  ${listing.startingPrice.toFixed(2)}
                </p>
                <span
                  className={`text-xs inline-block px-2 py-0.5 rounded-full font-medium w-fit ${
                    listing.status === "Active"
                      ? "bg-green-100 text-green-800"
                      : "bg-gray-100 text-gray-600"
                  }`}
                >
                  {listing.status}
                </span>
              </CardContent>
            </Card>
          ))}
        </div>

        {totalPages > 1 && (
          <div className="flex justify-center gap-2 mt-8">
            <Button
              variant="outline"
              disabled={page === 1}
              onClick={() => setPage((p) => Math.max(p - 1, 1))}
            >
              Previous
            </Button>
            <span className="px-4 py-2 text-sm text-gray-600">
              Page {page} of {totalPages}
            </span>
            <Button
              variant="outline"
              disabled={page === totalPages}
              onClick={() => setPage((p) => Math.min(p + 1, totalPages))}
            >
              Next
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}
