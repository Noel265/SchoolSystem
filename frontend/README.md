# SchoolSystem — Frontend

React + TypeScript + Vite single-page app for the `SchoolSystem.API` backend.

## Stack

- React 19 + TypeScript
- Vite
- React Router (client-side routing + role-based route guards)
- Axios (JWT bearer auth via request interceptor)

## Prerequisites

- Node.js (Vite 8 expects Node `^20.19 || ^22.13 || >=24`)
- The `SchoolSystem.API` backend running (default `http://localhost:5222`)

## Setup

```bash
npm install
cp .env.example .env   # optional — adjust the API URL if needed
npm run dev            # starts Vite on http://localhost:5173
```

The backend's CORS policy already allows `http://localhost:5173`.

## Configuration

| Variable            | Default                       | Description           |
| ------------------- | ----------------------------- | --------------------- |
| `VITE_API_BASE_URL` | `http://localhost:5222/api`   | Base URL of the API   |

If unset, the default in `src/api/client.ts` is used.

## Scripts

- `npm run dev` — start the dev server
- `npm run build` — type-check and build for production (`dist/`)
- `npm run lint` — run ESLint
- `npm run preview` — preview the production build

## Structure

```
src/
  api/         axios client + typed service wrappers
  components/  Layout (sidebar), ProtectedRoute, Modal, shared UI
  context/     auth context + provider
  hooks/       useAuth
  pages/       Login, Register, Dashboard, Students, Classes,
               Teachers, Attendance, Grades, Report Cards
  types.ts     types mirroring the backend DTOs
```

## Roles

Routes are guarded by role (Admin / Teacher / Parent):

- **Admin** — full access (students, classes, teachers, attendance, grades, report cards)
- **Teacher** — students, classes, attendance, grades, report cards
- **Parent** — dashboard + report cards

## Note on Subjects

The backend has no `GET /api/subjects` endpoint (the `SubjectsController` is
marked obsolete), so the grade-entry screen takes a numeric **Subject ID** that
must match a row in the backend's `Subjects` table. Adding a subjects listing
endpoint would let this become a dropdown.
