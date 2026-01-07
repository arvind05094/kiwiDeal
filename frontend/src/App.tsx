import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AuthProvider } from "@/features/auth/AuthContext";
import LoginPage from "@/features/auth/LoginPage";
import RegisterPage from "@/features/auth/RegisterPage";
import ListingsPage from "@/features/listings/ListingsPage";
import CreateListingPage from "./features/listings/CreateListingPage";
import ListingDetailPage from "./features/listings/ListingDetailPage";
import EditListingPage from "./features/listings/EditListingPage";
import { ProtectedRoute } from "./shared/components/ProtectedRoute";
import MyListingsPage from "./features/listings/MyListingsPage";
import AuctionDetailPage from "./features/auctions/AuctionDetailPage";
import AuctionsPage from "./features/auctions/AuctionsPage";

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route element={<ProtectedRoute />}>
              <Route path="/listings" element={<ListingsPage />} />
            </Route>
            <Route path="*" element={<Navigate to="/listings" replace />} />
            <Route path="/listings/new" element={<CreateListingPage />} />
            <Route path="/listings/:id" element={<ListingDetailPage />} />
            <Route path="/listings/:id/edit" element={<EditListingPage />} />
            <Route path="/my-listings" element={<MyListingsPage />} />
            <Route path="/auctions" element={<AuctionsPage />} />
            <Route path="/auctions/:id" element={<AuctionDetailPage />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
