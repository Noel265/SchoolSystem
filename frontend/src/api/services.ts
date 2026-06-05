import { api } from "./client";
import type {
  AuthResponse,
  LoginPayload,
  RegisterPayload,
  Student,
  CreateStudentPayload,
  UpdateStudentPayload,
  SchoolClass,
  CreateClassPayload,
  UpdateClassPayload,
  Teacher,
  CreateTeacherPayload,
  UpdateTeacherPayload,
  AttendanceRecord,
  AttendanceSummary,
  BulkAttendancePayload,
  EnterGradePayload,
  GradeResponse,
  StudentLookup,
  ReportCard,
  Subject,
  CreateSubjectPayload,
} from "../types";

// ── Auth ────────────────────────────────────────────────────
export const authApi = {
  login: (payload: LoginPayload) =>
    api.post<AuthResponse>("/auth/login", payload).then((r) => r.data),
  register: (payload: RegisterPayload) =>
    api.post<AuthResponse>("/auth/register", payload).then((r) => r.data),
};

// ── Students ────────────────────────────────────────────────
export const studentsApi = {
  getAll: () => api.get<Student[]>("/students").then((r) => r.data),
  getById: (id: number) =>
    api.get<Student>(`/students/${id}`).then((r) => r.data),
  getByClass: (classId: number) =>
    api.get<Student[]>(`/students/class/${classId}`).then((r) => r.data),
  create: (payload: CreateStudentPayload) =>
    api.post<Student>("/students", payload).then((r) => r.data),
  update: (id: number, payload: UpdateStudentPayload) =>
    api.put<Student>(`/students/${id}`, payload).then((r) => r.data),
  deactivate: (id: number) =>
    api.delete(`/students/${id}`).then((r) => r.data),
};

// ── Classes ─────────────────────────────────────────────────
export const classesApi = {
  getAll: () => api.get<SchoolClass[]>("/classes").then((r) => r.data),
  getById: (id: number) =>
    api.get<SchoolClass>(`/classes/${id}`).then((r) => r.data),
  create: (payload: CreateClassPayload) =>
    api.post<SchoolClass>("/classes", payload).then((r) => r.data),
  update: (id: number, payload: UpdateClassPayload) =>
    api.put<SchoolClass>(`/classes/${id}`, payload).then((r) => r.data),
  remove: (id: number) => api.delete(`/classes/${id}`).then((r) => r.data),
};

// ── Teachers ────────────────────────────────────────────────
export const teachersApi = {
  getAll: () => api.get<Teacher[]>("/teachers").then((r) => r.data),
  getById: (id: number) =>
    api.get<Teacher>(`/teachers/${id}`).then((r) => r.data),
  create: (payload: CreateTeacherPayload) =>
    api.post<Teacher>("/teachers", payload).then((r) => r.data),
  update: (id: number, payload: UpdateTeacherPayload) =>
    api.put<Teacher>(`/teachers/${id}`, payload).then((r) => r.data),
  deactivate: (id: number) =>
    api.delete(`/teachers/${id}`).then((r) => r.data),
};

// ── Attendance ──────────────────────────────────────────────
export const attendanceApi = {
  getByClassAndDate: (classId: number, date: string) =>
    api
      .get<AttendanceRecord[]>(`/attendance/class/${classId}/date/${date}`)
      .then((r) => r.data),
  getByStudent: (studentId: number) =>
    api
      .get<AttendanceRecord[]>(`/attendance/student/${studentId}`)
      .then((r) => r.data),
  getStudentSummary: (studentId: number) =>
    api
      .get<AttendanceSummary>(`/attendance/student/${studentId}/summary`)
      .then((r) => r.data),
  bulkMark: (payload: BulkAttendancePayload) =>
    api
      .post<{ message: string }>("/attendance/bulk", payload)
      .then((r) => r.data),
};

// ── Grades / Report cards ──────────────────────────────────
export const gradesApi = {
  lookupStudent: (registrationNumber: string) =>
    api
      .get<StudentLookup>(`/grades/lookup/${registrationNumber}`)
      .then((r) => r.data),
  enterGrade: (payload: EnterGradePayload) =>
    api.post<GradeResponse>("/grades/enter", payload).then((r) => r.data),
  getReportCard: (
    registrationNumber: string,
    term: string,
    academicYear: string
  ) =>
    api
      .get<ReportCard>(
        `/grades/reportcard/${registrationNumber}/${term}/${academicYear}`
      )
      .then((r) => r.data),
};

// ── Subjects ──────────────────────────────────
export const subjectsApi = {
  getAll: () => api.get<Subject[]>("/grades/subjects").then((r) => r.data),
  create: (payload: CreateSubjectPayload) =>
    api.post<Subject>("/grades/subjects", payload).then((r) => r.data),
  remove: (id: number) =>
    api.delete(`/grades/subjects/${id}`).then((r) => r.data),
};