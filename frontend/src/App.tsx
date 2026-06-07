import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthProvider";
import { Layout } from "./components/Layout";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { DashboardPage } from "./pages/DashboardPage";
import { StudentsPage } from "./pages/StudentsPage";
import { ClassesPage } from "./pages/ClassesPage";
import { TeachersPage } from "./pages/TeachersPage";
import { AttendancePage } from "./pages/AttendancePage";
import { GradesPage } from "./pages/GradesPage";
import { ReportCardsPage } from "./pages/ReportCardsPage";
import { SubjectsPage } from "./pages/SubjectsPage";
import { FeesPage } from "./pages/FeesPage";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Authenticated area */}
          <Route element={<ProtectedRoute />}>
            <Route element={<Layout />}>
              <Route path="/" element={<DashboardPage />} />
              <Route path="/report-cards" element={<ReportCardsPage />} />

              <Route element={<ProtectedRoute roles={["Admin", "Teacher"]} />}>
                <Route path="/students" element={<StudentsPage />} />
                <Route path="/classes" element={<ClassesPage />} />
                <Route path="/attendance" element={<AttendancePage />} />
                <Route path="/grades" element={<GradesPage />} />
              </Route>

              <Route element={<ProtectedRoute roles={["Admin"]} />}>
                <Route path="/teachers" element={<TeachersPage />} />
                <Route path="/subjects" element={<SubjectsPage />} />
                <Route path="/fees" element={<FeesPage />} />
              </Route>
            </Route>
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
