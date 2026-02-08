import { useState, useEffect } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { profileApi } from "../api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { NZ_REGIONS } from "@/shared/types/common";

export function ProfileTab() {
  const queryClient = useQueryClient();

  const { data: profile, isLoading } = useQuery({
    queryKey: ["me"],
    queryFn: profileApi.getMyProfile,
  });

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [region, setRegion] = useState("");
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    if (profile) {
      setFirstName(profile.firstName);
      setLastName(profile.lastName);
      setRegion(profile.region);
    }
  }, [profile]);

  const mutation = useMutation({
    mutationFn: () => profileApi.updateProfile({ firstName, lastName, region }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["me"] });
      setSaved(true);
      setTimeout(() => setSaved(false), 2500);
    },
  });

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="bg-white border rounded-lg p-6 max-w-md space-y-5">
      <h2 className="text-lg font-semibold text-gray-900">Edit Profile</h2>

      <div className="space-y-2">
        <Label>First Name</Label>
        <Input
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
        />
      </div>

      <div className="space-y-2">
        <Label>Last Name</Label>
        <Input value={lastName} onChange={(e) => setLastName(e.target.value)} />
      </div>

      <div className="space-y-2">
        <Label>Email</Label>
        <Input
          value={profile?.email ?? ""}
          disabled
          className="bg-gray-50 text-gray-400"
        />
        <p className="text-xs text-gray-400">Email cannot be changed here.</p>
      </div>

      <div className="space-y-2">
        <Label>Region</Label>
        <Select value={region} onValueChange={setRegion}>
          <SelectTrigger>
            <SelectValue placeholder="Select region" />
          </SelectTrigger>
          <SelectContent>
            {NZ_REGIONS.map((r) => (
              <SelectItem key={r.value} value={r.value}>
                {r.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <Button
        onClick={() => mutation.mutate()}
        disabled={mutation.isPending}
        className="bg-gray-900 hover:bg-gray-800 text-white"
      >
        {mutation.isPending ? "Saving…" : saved ? "Saved!" : "Save Changes"}
      </Button>
    </div>
  );
}
