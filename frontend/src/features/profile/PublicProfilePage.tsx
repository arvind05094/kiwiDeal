import { useParams, Link, useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { profileApi } from "./api";
import { listingsApi } from "@/features/listings/api";
import { auctionsApi } from "@/features/auctions/api";
import { RatingStars } from "@/shared/components/RatingStars";
import { ListingCard } from "@/shared/components/ListingCard";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { NZ_REGIONS } from "@/shared/types/common";
import { MapPin, Calendar, Star, MessageCircle } from "lucide-react";
import { format } from "date-fns";
import { Button } from "@/components/ui/button";
import { startConversation } from "../messages/api";
import { useAuth } from "../auth/AuthContext";

export function PublicProfilePage() {
  const { id } = useParams<{ id: string }>();

  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const isOwnProfile = user?.id === id;

  const { data: profile, isLoading } = useQuery({
    queryKey: ["profile", id],
    queryFn: () => profileApi.getPublicProfile(id!),
    enabled: !!id,
  });

  const { data: listings } = useQuery({
    queryKey: ["listings", { sellerId: id, status: "Active" }],
    queryFn: () =>
      listingsApi.getListings({ pageNumber: 1, pageSize: 8, sellerId: id }),
    enabled: !!id,
  });

  const { data: auctions } = useQuery({
    queryKey: ["auctions", { sellerId: id }],
    queryFn: () => auctionsApi.getAuctions({ pageNumber: 1, pageSize: 4 }),
    enabled: !!id,
  });

  if (isLoading) return <LoadingSpinner />;
  if (!profile)
    return (
      <EmptyState
        title="User not found"
        description="This profile does not exist."
      />
    );

  const regionLabel =
    NZ_REGIONS.find((r) => r.value === profile.region)?.label ?? profile.region;

  return (
    <div className="max-w-5xl mx-auto space-y-8">
      {/* Profile header */}
      <div className="bg-white border rounded-lg p-6 flex gap-6 items-start justify-between w-full">
        {/* Left: Avatar */}
        <div className="h-20 w-20 rounded-full bg-gray-200 flex items-center justify-center text-2xl font-semibold text-gray-600 shrink-0">
          {profile.firstName[0]}
          {profile.lastName[0]}
        </div>

        {/* Center/Left-aligned content area */}
        <div className="space-y-2 flex-1">
          <h1 className="text-2xl font-semibold text-gray-900">
            {profile.firstName} {profile.lastName}
          </h1>
          <div className="flex flex-wrap gap-4 text-sm text-gray-500">
            <span className="flex items-center gap-1">
              <MapPin className="h-4 w-4" />
              {regionLabel}
            </span>
            <span className="flex items-center gap-1">
              <Calendar className="h-4 w-4" />
              Member since {format(new Date(profile.memberSince), "MMMM yyyy")}
            </span>
          </div>
          {profile.totalRatings > 0 ? (
            <div className="flex items-center gap-2">
              <RatingStars
                value={profile.averageRating ?? 0}
                readonly
                size="sm"
              />
              <span className="text-sm text-gray-500">
                {profile.averageRating?.toFixed(1)} ({profile.totalRatings}{" "}
                rating{profile.totalRatings !== 1 ? "s" : ""})
              </span>
            </div>
          ) : (
            <span className="text-sm text-gray-400 block">No ratings yet</span>
          )}
        </div>

        {/* Right: Action Button Panel */}
        {isAuthenticated && !isOwnProfile && (
          <div className="self-start ml-auto shrink-0 pt-1">
            {/* Added pt-1 just to perfectly align it with the baseline of the name heading */}
            <Button
              variant="outline"
              size="sm"
              className="flex items-center gap-1.5"
              onClick={async () => {
                try {
                  const conversation = await startConversation({
                    recipientId: id!,
                    recipientName: `${profile.firstName} ${profile.lastName}`,
                    initialMessage: "Hi, I'd like to get in touch.",
                  });
                  navigate(`/messages/${conversation.id}`);
                } catch {}
              }}
            >
              <MessageCircle className="h-4 w-4" />
              Message
            </Button>
          </div>
        )}
      </div>
      {/* Active listings */}
      <section>
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Active Listings
        </h2>
        {listings?.items.length ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {listings.items.map((l) => (
              <ListingCard key={l.id} listing={l} />
            ))}
          </div>
        ) : (
          <EmptyState
            title="No active listings"
            description="This seller has no active listings."
          />
        )}
      </section>

      {/* Active auctions */}
      <section>
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Active Auctions
        </h2>
        {auctions?.items.length ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            {auctions.items.map((a) => (
              <Link
                key={a.id}
                to={`/auctions/${a.id}`}
                className="bg-white border rounded-lg p-4 hover:shadow-md transition-shadow"
              >
                <p className="font-medium text-gray-900 truncate">
                  {a.listingTitle}
                </p>
                <p className="text-sm text-gray-500 mt-1">
                  Current bid:{" "}
                  {a.currentHighestBid != null
                    ? `$${a.currentHighestBid.toFixed(2)}`
                    : `$${a.startingPrice.toFixed(2)}`}
                </p>
                <p className="text-xs text-gray-400 mt-1">
                  Ends {format(new Date(a.endTime), "dd MMM yyyy HH:mm")}
                </p>
              </Link>
            ))}
          </div>
        ) : (
          <EmptyState
            title="No active auctions"
            description="This seller has no active auctions."
          />
        )}
      </section>
    </div>
  );
}
