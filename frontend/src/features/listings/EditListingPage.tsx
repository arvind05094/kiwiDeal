import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "./api";
import Navbar from "@/shared/components/Navbar";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";

export default function EditListingPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const { data, isLoading } = useQuery({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getListing(id!),
  });

  const listing = data?.data;

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [startingPrice, setStartingPrice] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await listingsApi.updateListing(id!, {
        title: title || listing?.title || "",
        description: description || listing?.description || "",
        startingPrice: parseFloat(
          startingPrice || listing?.startingPrice.toString() || "0",
        ),
      });
      navigate(`/listings/${id}`);
    } catch {
      setError("Failed to update listing. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  if (isLoading)
    return (
      <div className="min-h-screen bg-gray-100">
        <Navbar />
        <p className="text-center py-12 text-gray-500">Loading...</p>
      </div>
    );

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-2xl mx-auto px-4 py-8">
        <Card className="bg-white shadow-md">
          <CardHeader>
            <CardTitle className="text-xl text-gray-700">
              Edit Listing
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-1">
                <Label htmlFor="title">Title</Label>
                <Input
                  id="title"
                  value={title || listing?.title || ""}
                  onChange={(e) => setTitle(e.target.value)}
                  required
                />
              </div>
              <div className="space-y-1">
                <Label htmlFor="description">Description</Label>
                <textarea
                  id="description"
                  value={description || listing?.description || ""}
                  onChange={(e) => setDescription(e.target.value)}
                  required
                  rows={5}
                  className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                />
              </div>
              <div className="space-y-1">
                <Label htmlFor="startingPrice">Starting Price (NZD)</Label>
                <Input
                  id="startingPrice"
                  type="number"
                  min="0"
                  step="0.01"
                  value={
                    startingPrice || listing?.startingPrice.toString() || ""
                  }
                  onChange={(e) => setStartingPrice(e.target.value)}
                  required
                />
              </div>
              {error && <p className="text-sm text-red-500">{error}</p>}
              <div className="flex gap-3 pt-2">
                <Button
                  type="submit"
                  className="bg-orange-500 hover:bg-orange-600 text-white"
                  disabled={loading}
                >
                  {loading ? "Saving..." : "Save Changes"}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate(`/listings/${id}`)}
                >
                  Cancel
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
