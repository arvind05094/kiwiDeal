import { useParams, useNavigate, Link } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listingsApi } from "./api";
import { ImageGallery } from "@/shared/components/ImageGallery";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { Button } from "@/components/ui/button";
import { MapPin, Tag, ArrowLeft, Heart, User } from "lucide-react";
import { createBuyNowCheckout, getPaymentByListing } from "../payments/api";
import { startConversation } from "../messages/api";
import type { ConversationDto } from "@/shared/types/common";
import { useListingHub } from "./useListingHub";
import { format } from "date-fns";
import { useAuth } from "../auth/useAuth";

export function ListingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const queryClient = useQueryClient();

  const { data: listing, isLoading } = useQuery({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getListing(id!),
    enabled: !!id,
  });

  const { data: isWatchedData } = useQuery({
    queryKey: ["listing-watched", id],
    queryFn: () => listingsApi.isWatched(id!),
    enabled: !!id && isAuthenticated,
  });
  const watched = isWatchedData ?? false;
  useListingHub(id!, () =>
    queryClient.invalidateQueries({ queryKey: ["listing", id] }),
  );
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
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listing-watched", id] });
      queryClient.invalidateQueries({ queryKey: ["listings-watchlist"] });
      queryClient.invalidateQueries({ queryKey: ["watchlist", "listings"] });
    },
  });

  const isSeller = user?.id === listing?.sellerId;

  const { data: buyerPayment } = useQuery({
    queryKey: ["payment-listing", id],
    queryFn: () => getPaymentByListing(id!),
    enabled: !!id && isAuthenticated && !isSeller && listing?.status === "Sold",
  });

  if (isLoading) return <LoadingSpinner />;
  if (!listing)
    return (
      <div className="text-center py-24 text-gray-400">Listing not found.</div>
    );

  const isActive = listing.status === "Active";
  const isFixedPrice = listing.listingType === "FixedPrice";
  const isAuction = listing.listingType === "Auction";

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Back */}
      <button
        onClick={() => navigate(-1)}
        className="inline-flex items-center gap-1.5 text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors"
      >
        <ArrowLeft className="h-4 w-4" /> Back to listings
      </button>

      <div className="grid grid-cols-1 lg:grid-cols-[5fr_4fr] gap-8 items-start">
        {/* Left — images */}
        <ImageGallery images={listing.images} title={listing.title} />

        {/* Right — all details */}
        <div className="space-y-5">
          {/* Title + status */}
          <div>
            <div className="flex items-start justify-between gap-3 mb-2">
              <h1 className="text-xl font-semibold text-gray-900 leading-snug">
                {listing.title}
              </h1>
              <StatusBadge status={listing.status} />
            </div>
            {buyerPayment?.status === "Completed" && buyerPayment.paidAt && (
              <p className="text-sm text-blue-600 font-medium mt-1">
                You purchased this on{" "}
                {format(new Date(buyerPayment.paidAt), "dd MMM yyyy")}
              </p>
            )}
            <div className="flex items-center gap-4 text-sm text-gray-500">
              <span className="flex items-center gap-1">
                <MapPin className="h-3.5 w-3.5" />
                {listing.region}
              </span>
              <span className="flex items-center gap-1">
                <Tag className="h-3.5 w-3.5" />
                {listing.category}
              </span>
            </div>
          </div>

          {/* Price */}
          {isFixedPrice && listing.buyNowPrice !== undefined && (
            <div className="py-3 border-t border-b border-gray-100">
              <p className="text-3xl font-bold text-gray-900">
                ${listing.buyNowPrice.toLocaleString()}
              </p>
              <p className="text-xs text-gray-400 mt-0.5">Fixed price</p>
            </div>
          )}

          {isAuction && (
            <div className="py-3 border-t border-b border-gray-100">
              <p className="text-sm text-gray-500">Auction listing</p>
              <p className="text-xs text-gray-400 mt-0.5">
                View auction for current bid
              </p>
            </div>
          )}
          {isAuction && listing.auctionId && (
            <Button
              asChild
              className="w-full bg-gray-900 hover:bg-gray-800 text-white"
            >
              <Link to={`/auctions/${listing.auctionId}`}>View Auction</Link>
            </Button>
          )}

          {/* Actions */}
          <div className="space-y-2">
            {isSeller && isActive && (
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  className="flex-1"
                  onClick={() => navigate(`/listings/${id}/edit`)}
                >
                  Edit Listing
                </Button>
                <Button
                  variant="outline"
                  className="flex-1 border-red-200 text-red-600 hover:bg-red-50"
                  onClick={() => cancelMutation.mutate()}
                  disabled={cancelMutation.isPending}
                >
                  {cancelMutation.isPending ? "Cancelling..." : "Cancel"}
                </Button>
              </div>
            )}

            {!isSeller && isActive && (
              <div className="space-y-2">
                {isFixedPrice && listing.buyNowPrice !== undefined && (
                  <Button
                    className="w-full bg-orange-500 hover:bg-orange-600 text-white"
                    onClick={async () => {
                      try {
                        const { checkoutUrl } = await createBuyNowCheckout({
                          listingId: listing.id,
                          sellerId: listing.sellerId,
                          amount: listing.buyNowPrice!,
                        });
                        window.location.href = checkoutUrl;
                      } catch {
                        //intentional
                      }
                    }}
                  >
                    Buy Now — ${listing.buyNowPrice.toLocaleString()}
                  </Button>
                )}

                {isAuthenticated && (
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      className="flex-1"
                      onClick={() => watchMutation.mutate()}
                      disabled={watchMutation.isPending}
                    >
                      <Heart
                        className={`h-4 w-4 mr-1.5 ${watched ? "fill-orange-500 text-orange-500" : ""}`}
                      />
                      {watched ? "Watching" : "Watch"}
                    </Button>
                    <Button
                      variant="outline"
                      className="flex-1"
                      onClick={async () => {
                        try {
                          const conversation = await startConversation({
                            recipientId: listing.sellerId,
                            recipientName: listing.sellerName,
                            initialMessage:
                              "Hi, I'm interested in this listing.",
                          });
                          queryClient.setQueryData<ConversationDto[]>(
                            ["conversations"],
                            (prev) =>
                              prev ? [conversation, ...prev] : [conversation],
                          );
                          navigate(`/messages/${conversation.id}`);
                        } catch {
                          //intentional
                        }
                      }}
                    >
                      Message Seller
                    </Button>
                  </div>
                )}
              </div>
            )}

            {!isActive && (
              <p className="text-sm text-gray-400 italic bg-gray-50 rounded-lg px-4 py-3">
                This listing is {listing.status.toLowerCase()} and no longer
                available.
              </p>
            )}
          </div>

          {/* Seller */}
          <div className="flex items-center justify-between pt-4 border-t border-gray-100">
            {/* Added items-center here so avatar and text stay vertically centered */}
            <div className="flex items-center gap-2.5">
              <div className="h-8 w-8 rounded-full bg-gray-100 flex items-center justify-center shrink-0">
                <User className="h-4 w-4 text-gray-400" />
              </div>
              {/* Explicit leading-none or alignment prevents the paragraph tag from introducing default text margins */}
              <div className="flex flex-col gap-0.5">
                <p className="text-xs text-gray-400 leading-none">Listed by</p>
                <p className="text-sm font-medium text-gray-900 leading-none">
                  {listing.sellerName}
                  {isSeller && (
                    <span className="text-gray-400 font-normal"> (You)</span>
                  )}
                </p>
              </div>
            </div>

            <Link
              to={`/users/${listing.sellerId}`}
              className="inline-flex items-center gap-1 text-sm font-medium text-gray-900 border border-gray-200 rounded-lg px-3 h-8 hover:bg-gray-50 transition-colors"
            >
              View profile <ArrowLeft className="h-3.5 w-3.5 rotate-180" />
            </Link>
          </div>

          {/* Description */}
          <div className="pt-1 border-t border-gray-100">
            <h2 className="text-sm font-semibold text-gray-700 mb-2">
              Description
            </h2>
            <p className="text-sm text-gray-600 whitespace-pre-line leading-relaxed">
              {listing.description}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
