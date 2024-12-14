using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SCMS_back_end.Repositories.Services
{
    public class SubjectService : ISubject
    {
        private readonly StudyCenterDbContext _context;

        public SubjectService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<DtoSubjectResponse> AddSubjectAsync(DtoSubjectRequest subjectDto)
        {
            var subject = new Subject
            {
                Name = subjectDto.Name,
                DepartmentId = subjectDto.DepartmentId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return new DtoSubjectResponse
            {
                SubjectId = subject.SubjectId,
                Name = subject.Name,
                DepartmentId = subject.DepartmentId
            };
        }
        
        public async Task<IEnumerable<DtoSubjectResponse>> GetAllSubjectsAsync()
        {
            var subjects= await _context.Subjects.ToListAsync();
            var subjectsDto = subjects.Select(s => new DtoSubjectResponse
            {
                SubjectId = s.SubjectId,
                Name = s.Name,
                DepartmentId = s.DepartmentId
            }).ToList();
            return subjectsDto;
        }

        public async Task<DtoSubjectResponse> GetSubjectByIdAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if(subject != null)
            {
                return new DtoSubjectResponse
                {
                    SubjectId = subject.SubjectId,
                    Name = subject.Name,
                    DepartmentId = subject.DepartmentId
                };
            }
            return null; 
        }

        public async Task<DtoSubjectResponse> UpdateSubjectAsync(int id,DtoSubjectRequest subjectDto)
        {
            var existingSubject = await _context.Subjects.FindAsync(id);
            if (existingSubject != null)
            {
                existingSubject.Name = subjectDto.Name;
                existingSubject.DepartmentId = subjectDto.DepartmentId;
                await _context.SaveChangesAsync();

                return new DtoSubjectResponse
                {
                    SubjectId = existingSubject.SubjectId,
                    Name = existingSubject.Name,
                    DepartmentId = existingSubject.DepartmentId
                };
            }
            return null;
        }
    }
}
