import { useParams, useNavigate, Link } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listingsApi } from "./api";
import { ImageGallery } from "@/shared/components/ImageGallery";
import { SellerInfo } from "@/shared/components/SellerInfo";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/shared/hooks/useAuth";
import { MapPin, Tag, ArrowLeft, Heart } from "lucide-react";
import { useState } from "react";

export function ListingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const queryClient = useQueryClient();
  const [watched, setWatched] = useState(false);

  const { data: listing, isLoading } = useQuery({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getListing(id!),
    enabled: !!id,
  });

  const cancelMutation = useMutation({
    mutationFn: () => listingsApi.cancelListing(id!),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["listing", id] }),
  });

  const watchMutation = useMutation({
    mutationFn: () =>
      watched
        ? listingsApi.removeFromWatchlist(id!)
        : listingsApi.addToWatchlist(id!),
    onSuccess: () => setWatched((w) => !w),
  });

  if (isLoading) return <LoadingSpinner />;
  if (!listing)
    return (
      <div className="text-center py-24 text-gray-400">Listing not found.</div>
    );

  const isSeller = user?.id === listing.sellerId;
  const isActive = listing.status === "Active";
  const isFixedPrice = listing.listingType === "FixedPrice";
  const isAuction = listing.listingType === "Auction";

  return (
    <div className="space-y-6">
      {/* Back */}
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-1 text-sm text-gray-500 hover:text-gray-800 transition-colors"
      >
        <ArrowLeft className="h-4 w-4" /> Back
      </button>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Left — images */}
        <ImageGallery images={listing.images} title={listing.title} />

        {/* Right — details */}
        <div className="space-y-5">
          <div className="flex items-start justify-between gap-3">
            <h1 className="text-2xl font-semibold text-gray-900">
              {listing.title}
            </h1>
            <StatusBadge status={listing.status} />
          </div>

          <div className="flex items-center gap-4 text-sm text-gray-500">
            <span className="flex items-center gap-1">
              <MapPin className="h-4 w-4" />
              {listing.region}
            </span>
            <span className="flex items-center gap-1">
              <Tag className="h-4 w-4" />
              {listing.category}
            </span>
          </div>

          {isFixedPrice && listing.buyNowPrice !== undefined && (
            <p className="text-3xl font-bold text-gray-900">
              ${listing.buyNowPrice.toLocaleString()}
            </p>
          )}

          {isAuction && (
            <p className="text-sm text-gray-500 italic">
              See auction for current bid
            </p>
          )}

          {/* Seller info — placeholder until users api is wired */}
          <div className="rounded-lg border border-gray-100 p-4 bg-gray-50">
            <p className="text-xs text-gray-400 mb-2">Seller</p>
            <Link
              to={`/users/${listing.sellerId}`}
              className="text-sm font-medium text-gray-900 hover:text-orange-500 transition-colors"
            >
              View seller profile →
            </Link>
          </div>

          {/* Actions */}
          <div className="flex flex-wrap gap-3">
            {/* Seller actions */}
            {isSeller && isActive && (
              <>
                <Button
                  variant="outline"
                  onClick={() => navigate(`/listings/${id}/edit`)}
                >
                  Edit
                </Button>
                <Button
                  variant="outline"
                  className="border-red-300 text-red-600 hover:bg-red-50"
                  onClick={() => cancelMutation.mutate()}
                  disabled={cancelMutation.isPending}
                >
                  {cancelMutation.isPending
                    ? "Cancelling..."
                    : "Cancel Listing"}
                </Button>
                {isAuction && (
                  <Button
                    onClick={() =>
                      navigate(`/auctions/new?listingId=${listing.id}`)
                    }
                  >
                    Start Auction
                  </Button>
                )}
              </>
            )}

            {/* Buyer actions */}
            {!isSeller && isActive && (
              <>
                {isFixedPrice && (
                  <Button className="bg-orange-500 hover:bg-orange-600 text-white">
                    Buy Now
                  </Button>
                )}
                {isAuction && (
                  <Button asChild>
                    <Link to={`/auctions/${id}`}>View Auction</Link>
                  </Button>
                )}
                {isAuthenticated && (
                  <Button
                    variant="outline"
                    onClick={() => watchMutation.mutate()}
                    disabled={watchMutation.isPending}
                    className="flex items-center gap-1"
                  >
                    <Heart
                      className={`h-4 w-4 ${watched ? "fill-orange-500 text-orange-500" : ""}`}
                    />
                    {watched ? "Watching" : "Watch"}
                  </Button>
                )}
                {isAuthenticated && (
                  <Button variant="outline" asChild>
                    <Link
                      to={`/messages/new?listingId=${listing.id}&sellerId=${listing.sellerId}`}
                    >
                      Message Seller
                    </Link>
                  </Button>
                )}
              </>
            )}
          </div>

          {!isActive && (
            <p className="text-sm text-gray-400 italic">
              This listing is {listing.status.toLowerCase()} and no longer
              available.
            </p>
          )}
        </div>
      </div>

      {/* Description */}
      <div className="space-y-2">
        <h2 className="text-base font-semibold text-gray-900">Description</h2>
        <p className="text-sm text-gray-600 whitespace-pre-line leading-relaxed">
          {listing.description}
        </p>
      </div>
    </div>
  );
}
