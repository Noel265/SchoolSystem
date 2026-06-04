// Shared types mirroring the backend DTOs (SchoolSystem.API/DTOs)

export type Role = "Admin" | "Teacher" | "Parent";

export interface AuthResponse {
  token: string;
  fullName: string;
  email: string;
  role: Role;
  expiresAt: string;
}

export interface RegisterPayload {
  fullName: string;
  email: string;
  password: string;
  role: Role;
}

export interface LoginPayload {
  email: string;
  password: string;
}

// ── Students ────────────────────────────────────────────────
export interface Student {
  id: number;
  fullName: string;
  admissionNumber: string;
  dateOfBirth: string; // DateOnly -> "yyyy-MM-dd"
  gender: string;
  schoolLevel: string;
  className: string;
  enrollmentDate: string;
  isActive: boolean;
}

export interface CreateStudentPayload {
  fullName: string;
  admissionNumber: string;
  dateOfBirth: string;
  gender: string;
  schoolLevel: string;
  classId: number;
}

export interface UpdateStudentPayload {
  fullName: string;
  dateOfBirth: string;
  gender: string;
  schoolLevel: string;
  classId: number;
}

// ── Classes ─────────────────────────────────────────────────
export interface SchoolClass {
  id: number;
  name: string;
  schoolLevel: string;
  academicYear: string;
  classTeacherName?: string | null;
  studentCount: number;
}

export interface CreateClassPayload {
  name: string;
  schoolLevel: string;
  academicYear: string;
  classTeacherId?: number | null;
}

export interface UpdateClassPayload {
  name: string;
  schoolLevel: string;
  academicYear: string;
  classTeacherId?: number | null;
}

// ── Teachers ────────────────────────────────────────────────
export interface Teacher {
  id: number;
  fullName: string;
  employeeNumber: string;
  phoneNumber: string;
  specialization: string;
  hireDate: string;
  email: string;
  isActive: boolean;
}

export interface CreateTeacherPayload {
  fullName: string;
  employeeNumber: string;
  phoneNumber: string;
  specialization: string;
  hireDate: string;
  email: string;
  password: string;
}

export interface UpdateTeacherPayload {
  fullName: string;
  phoneNumber: string;
  specialization: string;
}

// ── Attendance ──────────────────────────────────────────────
export type AttendanceStatus = "Present" | "Absent" | "Late";

export interface AttendanceRecord {
  id: number;
  studentName: string;
  admissionNumber: string;
  date: string;
  status: string;
  remarks?: string | null;
  teacherName: string;
}

export interface AttendanceSummary {
  studentId: number;
  studentName: string;
  totalDays: number;
  presentDays: number;
  absentDays: number;
  lateDays: number;
  attendancePercentage: number;
}

export interface StudentAttendanceItem {
  studentId: number;
  status: string;
  remarks?: string | null;
}

export interface BulkAttendancePayload {
  classId: number;
  date: string;
  attendances: StudentAttendanceItem[];
}

// ── Grades / Report cards ───────────────────────────────────
export interface EnterGradePayload {
  registrationNumber: string;
  subjectId: number;
  midtermScore: number;
  finalScore: number;
  term: string;
  academicYear: string;
  comments?: string | null;
}

export interface GradeResponse {
  id: number;
  studentName: string;
  registrationNumber: string;
  subjectName: string;
  midtermScore: number;
  finalScore: number;
  totalScore: number;
  status: string;
  term: string;
  academicYear: string;
  comments?: string | null;
  teacherName: string;
}

export interface StudentLookup {
  id: number;
  fullName: string;
  admissionNumber: string;
  age: number;
  className: string;
  schoolLevel: string;
}

export interface ReportCardSubject {
  subjectName: string;
  midtermScore: number;
  finalScore: number;
  subjectTotal: number;
  classMidtermAverage: number;
  classFinalAverage: number;
  classSubjectAverage: number;
  status: string;
  comments?: string | null;
  teacherName: string;
}

export interface ReportCard {
  studentName: string;
  registrationNumber: string;
  className: string;
  age: number;
  term: string;
  academicYear: string;
  subjects: ReportCardSubject[];
  midtermTotal: number;
  finalTotal: number;
  overallAverage: number;
  position: number;
  totalStudentsInClass: number;
  overallStatus: string;
}

export interface ApiError {
  message?: string;
}
