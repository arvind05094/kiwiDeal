import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { registerApi } from "@/features/auth/api";
import { setAccessToken } from "@/shared/api/client";
import { NZ_REGIONS } from "@/shared/types/common";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAuth } from "./useAuth";

export function RegisterPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [region, setRegion] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.SyntheticEvent) => {
    e.preventDefault();
    setError(null);
    setFieldErrors({});
    setLoading(true);
    try {
      const data = await registerApi({
        firstName,
        lastName,
        email,
        password,
        region,
      });
      setAccessToken(data.accessToken);
      await login(email, password);
      navigate("/listings", { replace: true });
    } catch (err: unknown) {
      const e = err as {
        response?: {
          data?: { errors?: Record<string, string[]>; detail?: string };
        };
      };
      if (e.response?.data?.errors) {
        setFieldErrors(e.response.data.errors);
      } else {
        setError(
          e.response?.data?.detail ?? "Registration failed. Please try again.",
        );
      }
    } finally {
      setLoading(false);
    }
  };

  const fieldError = (key: string) => fieldErrors[key]?.[0] ?? null;

  return (
    <div className="flex min-h-[70vh] items-center justify-center py-8">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle className="text-xl">Create an account</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label htmlFor="firstName">First name</Label>
                <Input
                  id="firstName"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  required
                />
                {fieldError("FirstName") && (
                  <p className="text-xs text-red-600">
                    {fieldError("FirstName")}
                  </p>
                )}
              </div>
              <div className="space-y-1">
                <Label htmlFor="lastName">Last name</Label>
                <Input
                  id="lastName"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  required
                />
                {fieldError("LastName") && (
                  <p className="text-xs text-red-600">
                    {fieldError("LastName")}
                  </p>
                )}
              </div>
            </div>
            <div className="space-y-1">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
              {fieldError("Email") && (
                <p className="text-xs text-red-600">{fieldError("Email")}</p>
              )}
            </div>
            <div className="space-y-1">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                autoComplete="new-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
              {fieldError("Password") && (
                <p className="text-xs text-red-600">{fieldError("Password")}</p>
              )}
            </div>
            <div className="space-y-1">
              <Label htmlFor="region">Region</Label>
              <Select value={region} onValueChange={setRegion} required>
                <SelectTrigger id="region">
                  <SelectValue placeholder="Select your region" />
                </SelectTrigger>
                <SelectContent>
                  {NZ_REGIONS.map((r) => (
                    <SelectItem key={r.value} value={r.value}>
                      {r.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {fieldError("Region") && (
                <p className="text-xs text-red-600">{fieldError("Region")}</p>
              )}
            </div>
            {error && <p className="text-sm text-red-600">{error}</p>}
            <Button
              type="submit"
              className="w-full bg-[#1a1a1a] hover:bg-gray-800 text-white active:scale-95 transition-all duration-150"
              disabled={loading || !region}
            >
              {loading ? "Creating account…" : "Register"}
            </Button>
          </form>
          <p className="mt-4 text-center text-sm text-gray-500">
            Already have an account?{" "}
            <Link to="/login" className="text-orange-500 hover:underline">
              Sign in
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
