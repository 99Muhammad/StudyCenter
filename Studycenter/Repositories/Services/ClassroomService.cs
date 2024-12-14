using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class ClassroomService : IClassroom
    {
        private readonly StudyCenterDbContext _context;

        public ClassroomService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<DtoClassroomResponse> AddClassroomAsync(DtoCreateClassroomRequest classroom)
        {
            if (classroom == null) { return null; }
            var newClassroom = new Classroom
            {
                RoomNumber = classroom.RoomNumber,
                Capacity = classroom.Capacity
            };
            await _context.Classrooms.AddAsync(newClassroom);
            await _context.SaveChangesAsync();
            var classroomResponse = new DtoClassroomResponse
            {
                ClassroomId = newClassroom.ClassroomId,
                RoomNumber = newClassroom.RoomNumber,
                Capacity = newClassroom.Capacity
            };
            return classroomResponse;
        }

        public async Task<bool> DeleteClassroomAsync(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
            {
                return false;
            }
            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DtoClassroomResponse>> GetAllClassroomsAsync()
        {
            var classrooms = await _context.Classrooms.ToListAsync();
            var classroomResponses = classrooms.Select(classroom => new DtoClassroomResponse
            {
                ClassroomId = classroom.ClassroomId,
                RoomNumber = classroom.RoomNumber,
                Capacity = classroom.Capacity
            });
            return classroomResponses;
        }

        public async Task<DtoClassroomResponse> GetClassroomByIdAsync(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null)
            {
                return null;
            }
            var classroomResponse = new DtoClassroomResponse
            {
                ClassroomId = classroom.ClassroomId,
                RoomNumber = classroom.RoomNumber,
                Capacity = classroom.Capacity
            };
            return classroomResponse;
        }

        public async Task<DtoClassroomResponse> UpdateClassroomAsync(int id, DtoUpdateClassroomRequest classroom)
        {
            var classroomToUpdate = await _context.Classrooms.FindAsync(id);
            if (classroomToUpdate == null)
            {
                return null;
            }
            classroomToUpdate.RoomNumber = classroom.RoomNumber;
            classroomToUpdate.Capacity = classroom.Capacity;
            await _context.SaveChangesAsync();
            var classroomResponse = new DtoClassroomResponse
            {
                ClassroomId = classroomToUpdate.ClassroomId,
                RoomNumber = classroomToUpdate.RoomNumber,
                Capacity = classroomToUpdate.Capacity
            };
            return classroomResponse;
        }
    }
}
