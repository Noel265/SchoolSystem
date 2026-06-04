import api from './api';

export const gradesService = {
  // Student lookup by registration number
  lookupStudent: (registrationNumber) =>
    api.get(`/grades/lookup/${registrationNumber}`),

  // Enter grade (midterm + final)
  enterGrade: (gradeData) =>
    api.post('/grades/enter', gradeData),

  // Get report card
  getReportCard: (registrationNumber, term, academicYear) =>
    api.get(`/grades/reportcard/${registrationNumber}/${term}/${academicYear}`),

  // Get student grades (legacy)
  getStudentGrades: (admissionNumber, term, academicYear) =>
    api.get(`/grades/student/${admissionNumber}`, {
      params: { term, academicYear }
    }),

  // Get class grades
  getClassGrades: (classId, term, academicYear) =>
    api.get(`/grades/class/${classId}`, {
      params: { term, academicYear }
    }),
};
