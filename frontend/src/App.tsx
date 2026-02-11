import { Routes, Route, Navigate } from "react-router-dom";
import { Navbar } from "@/shared/components/Navbar";
import { ProtectedRoute } from "@/shared/components/ProtectedRoute";
import { LoginPage } from "@/features/auth/LoginPage";
import { RegisterPage } from "@/features/auth/RegisterPage";
import { ListingsPage } from "@/features/listings/ListingsPage";
import { ListingDetailPage } from "@/features/listings/ListingDetailPage";
import { CreateListingPage } from "@/features/listings/CreateListingPage";
import { EditListingPage } from "@/features/listings/EditListingPage";
import { AuctionDetailPage } from "@/features/auctions/AuctionDetailPage";
import { PublicProfilePage } from "@/features/profile/PublicProfilePage";
import { MyAccountPage } from "@/features/profile/MyAccountPage";
import { InboxPage } from "@/features/messages/InboxPage";
import { ConversationPage } from "@/features/messages/ConversationPage";

function PlaceholderPage({ name }: { name: string }) {
  return (
    <div className="flex items-center justify-center py-24 text-gray-400">
      {name} — coming soon
    </div>
  );
}

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <Routes>
          <Route path="/" element={<Navigate to="/listings" replace />} />
          <Route path="/listings" element={<ListingsPage />} />
          <Route
            path="/listings/new"
            element={
              <ProtectedRoute>
                <CreateListingPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/listings/:id/edit"
            element={
              <ProtectedRoute>
                <EditListingPage />
              </ProtectedRoute>
            }
          />
          <Route path="/listings/:id" element={<ListingDetailPage />} />
          <Route path="/auctions/:id" element={<AuctionDetailPage />} />
          <Route path="/users/:id" element={<PublicProfilePage />} />
          <Route
            path="/account"
            element={
              <ProtectedRoute>
                <MyAccountPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/messages"
            element={
              <ProtectedRoute>
                <PlaceholderPage name="Inbox" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/messages/:conversationId"
            element={
              <ProtectedRoute>
                <PlaceholderPage name="Conversation" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/watchlist"
            element={
              <ProtectedRoute>
                <PlaceholderPage name="Watchlist" />
              </ProtectedRoute>
            }
          />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/messages"
            element={
              <ProtectedRoute>
                <InboxPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/messages/:conversationId"
            element={
              <ProtectedRoute>
                <ConversationPage />
              </ProtectedRoute>
            }
          />
        </Routes>
      </main>
    </div>
  );
}
