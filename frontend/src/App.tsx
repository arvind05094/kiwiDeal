import { Routes, Route, Navigate } from "react-router-dom";
import { Navbar } from "@/shared/components/Navbar";
import { ProtectedRoute } from "@/shared/components/ProtectedRoute";

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
          <Route
            path="/listings"
            element={<PlaceholderPage name="Listings" />}
          />
          <Route
            path="/listings/:id"
            element={<PlaceholderPage name="Listing Detail" />}
          />
          <Route
            path="/listings/new"
            element={
              <ProtectedRoute>
                <PlaceholderPage name="Create Listing" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/listings/:id/edit"
            element={
              <ProtectedRoute>
                <PlaceholderPage name="Edit Listing" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/auctions/:id"
            element={<PlaceholderPage name="Auction Detail" />}
          />
          <Route
            path="/users/:id"
            element={<PlaceholderPage name="Public Profile" />}
          />
          <Route
            path="/account"
            element={
              <ProtectedRoute>
                <PlaceholderPage name="My Account" />
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
          <Route path="/login" element={<PlaceholderPage name="Login" />} />
          <Route
            path="/register"
            element={<PlaceholderPage name="Register" />}
          />
        </Routes>
      </main>
    </div>
  );
}
