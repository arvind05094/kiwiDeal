import { useNavigate, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "./api";
import Navbar from "@/shared/components/Navbar";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { useAuth } from "@/features/auth/AuthContext";

export default function ListingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();

  const { data, isLoading, isError } = useQuery({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getListing(id!),
  });

  const listing = data?.data;
  const isSeller = user?.id === listing?.sellerId;

  if (isLoading)
    return (
      <div className="min-h-screen bg-gray-100">
        <Navbar />
        <p className="text-center py-12 text-gray-500">Loading...</p>
      </div>
    );

  if (isError || !listing)
    return (
      <div className="min-h-screen bg-gray-100">
        <Navbar />
        <p className="text-center py-12 text-red-500">Listing not found.</p>
      </div>
    );

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-3xl mx-auto px-4 py-8">
        <Button
          variant="outline"
          className="mb-4"
          onClick={() => navigate("/listings")}
        >
          ← Back to Listings
        </Button>
        <Card className="bg-white shadow-md">
          <div className="h-64 bg-gray-200 rounded-t-lg flex items-center justify-center overflow-hidden">
            {listing.images && listing.images.length > 0 ? (
              <img
                src={listing.images[0].url}
                alt={listing.title}
                className="object-cover w-full h-full"
              />
            ) : (
              <span className="text-gray-400">No image</span>
            )}
          </div>
          <CardContent className="p-6 space-y-4">
            <div className="flex items-start justify-between">
              <h1 className="text-2xl font-bold text-gray-800">
                {listing.title}
              </h1>
              <span
                className={`text-xs px-2 py-1 rounded-full font-medium ${
                  listing.status === "Active"
                    ? "bg-green-100 text-green-800"
                    : "bg-gray-100 text-gray-600"
                }`}
              >
                {listing.status}
              </span>
            </div>
            <p className="text-orange-500 text-2xl font-bold">
              ${listing.startingPrice.toFixed(2)}
            </p>
            <p className="text-gray-600 whitespace-pre-wrap">
              {listing.description}
            </p>
            <p className="text-xs text-gray-400">
              Listed{" "}
              {new Date(listing.createdAt).toLocaleDateString("en-NZ", {
                day: "numeric",
                month: "long",
                year: "numeric",
              })}
            </p>
            {isSeller && (
              <div className="flex gap-3 pt-2 border-t">
                <Button
                  className="bg-orange-500 hover:bg-orange-600 text-white"
                  onClick={() => navigate(`/listings/${listing.id}/edit`)}
                >
                  Edit Listing
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
