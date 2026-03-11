import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { listingsApi } from "./api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import type { ListingDto, ProblemDetails } from "@/shared/types/common";

export function EditListingPage() {
  const { id } = useParams<{ id: string }>();

  const { data: listing, isLoading } = useQuery({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getListing(id!),
    enabled: !!id,
  });

  if (isLoading) return <LoadingSpinner />;
  if (!listing)
    return (
      <div className="text-center py-24 text-gray-400">Listing not found.</div>
    );

  return <EditListingForm id={id!} listing={listing} />;
}

function EditListingForm({ id, listing }: { id: string; listing: ListingDto }) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [title, setTitle] = useState(listing.title);
  const [description, setDescription] = useState(listing.description);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});

  const updateMutation = useMutation({
    mutationFn: () => listingsApi.updateListing(id, { title, description }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listing", id] });
      navigate(`/listings/${id}`);
    },
    onError: (err: unknown) => {
      const data = (err as { response?: { data?: ProblemDetails } }).response
        ?.data;
      if (data?.errors) setFieldErrors(data.errors);
    },
  });

  const err = (field: string) => fieldErrors[field]?.[0];

  return (
    <div className="max-w-2xl mx-auto space-y-8">
      <h1 className="text-2xl font-semibold text-gray-900">Edit Listing</h1>

      {listing.listingType === "Auction" && (
        <p className="text-sm text-amber-600 bg-amber-50 border border-amber-200 rounded-lg px-4 py-3">
          This is an auction listing. Only the title and description can be
          edited.
        </p>
      )}

      <div className="space-y-5">
        <div className="space-y-1.5">
          <Label htmlFor="title">Title</Label>
          <Input
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
          {err("Title") && (
            <p className="text-xs text-red-500">{err("Title")}</p>
          )}
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="description">Description</Label>
          <Textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={6}
          />
          {err("Description") && (
            <p className="text-xs text-red-500">{err("Description")}</p>
          )}
        </div>
      </div>

      <div className="flex gap-3 pt-2">
        <Button
          className="bg-gray-900 hover:bg-gray-800 text-white flex-1"
          onClick={() => updateMutation.mutate()}
          disabled={updateMutation.isPending}
        >
          {updateMutation.isPending ? "Saving..." : "Save Changes"}
        </Button>
        <Button variant="outline" onClick={() => navigate(`/listings/${id}`)}>
          Cancel
        </Button>
      </div>
    </div>
  );
}
