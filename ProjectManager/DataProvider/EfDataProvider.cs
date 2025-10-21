using Microsoft.EntityFrameworkCore;
using ProjectManager.Helper;
using ProjectManager.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectManager.DataProvider
{
    public class EfDataProvider : IDataProvider
    {
        private readonly ProjectManagerContext _context;
        private User currentUser;

        public User CurrentUser => currentUser;

        public EfDataProvider(ProjectManagerContext context)
        {
            _context = context;
        }

        // ---------------- USERS ----------------

        public async Task<bool> CreateUser(User user)
        {
            try
            {
                user.Password = EncryptionHelper.EncryptString(user.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Benutzers: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                bool mustUpdateCurrentUser = user.Id == CurrentUser?.Id;
                user.Password = EncryptionHelper.EncryptString(user.Password);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                if (mustUpdateCurrentUser)
                    currentUser = await GetUser(user.Id);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren des Benutzers: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteUser(User user)
        {
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen des Benutzers: " + ex.Message);
                return false;
            }
        }

        public async Task<ObservableCollection<User>> GetUsers(UserFilter userFilter = UserFilter.No)
        {
            try
            {
                IQueryable<User> query = _context.Users;

                switch (userFilter)
                {
                    case UserFilter.Admin:
                        query = query.Where(u => u.IsAdmin);
                        break;
                    case UserFilter.User:
                        query = query.Where(u => !u.IsAdmin);
                        break;
                }

                var users = await query.ToListAsync();

                foreach (var u in users)
                    u.Password = EncryptionHelper.DecryptString(u.Password);

                return new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Benutzer: " + ex.Message);
                return new ObservableCollection<User>();
            }
        }

        public async Task<User> GetUser(User user)
        {
            try
            {
                var fullUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                if (fullUser != null)
                    fullUser.Password = EncryptionHelper.DecryptString(fullUser.Password);

                return fullUser;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Benutzers: " + ex.Message);
                return null;
            }
        }

        public async Task<User> GetUser(int id)
        {
            try
            {
                var fullUser = await _context.Users.FindAsync(id);

                if (fullUser != null)
                    fullUser.Password = EncryptionHelper.DecryptString(fullUser.Password);

                return fullUser;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Benutzers: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> Login(User user)
        {
            try
            {
                var fullUser = await GetUser(user);
                if (fullUser == null) return false;

                if (user.Password == fullUser.Password)
                {
                    currentUser = fullUser;
                    ObjectRepository.AppConfiguration.CurrentUser = fullUser;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        // ---------------- PROJECTS ----------------

        public async Task<bool> CreateProject(Project project)
        {
            try
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Projekts: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateProject(Project project)
        {
            try
            {
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren des Projekts: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteProject(Project project)
        {
            try
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen des Projekts: " + ex.Message);
                return false;
            }
        }

        public async Task<ObservableCollection<Project>> GetProjects()
            => await GetProjects(ProjectFilter.No);

        public async Task<ObservableCollection<Project>> GetProjects(ProjectFilter filter, User user = null)
        {
            try
            {
                IQueryable<Project> query = _context.Projects.Include(p => p.Responsibility);

                switch (filter)
                {
                    case ProjectFilter.MyProjects:
                        query = query.Where(p => p.Responsibility.Id == currentUser.Id);
                        break;
                    case ProjectFilter.ProjectFrom:
                        query = query.Where(p => p.Responsibility.Id == user.Id);
                        break;
                }

                var projects = await query.OrderBy(p => p.Title).ToListAsync();
                return new ObservableCollection<Project>(projects);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Projekte: " + ex.Message);
                return new ObservableCollection<Project>();
            }
        }

        public async Task<Project> GetProject(int id)
        {
            try
            {
                return await _context.Projects
                    .Include(p => p.Responsibility)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Projekts: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> IsMyProject(Project project)
        {
            return project.Responsibility.Id == currentUser.Id;
        }

        // ---------------- ASSIGNMENTS ----------------

        public async Task<bool> CreateAssignment(Assignment assignment)
        {
            try
            {
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen der Aufgabe: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAssignment(Assignment assignment)
        {
            try
            {
                _context.Assignments.Update(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren der Aufgabe: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAssignment(Assignment assignment)
        {
            try
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen der Aufgabe: " + ex.Message);
                return false;
            }
        }

        public async Task<ObservableCollection<Assignment>> GetAssignments(Project project)
        {
            try
            {
                var assignments = await _context.Assignments
                    .Include(a => a.Project)
                    .Include(a => a.User)
                    .Where(a => a.Project.Id == project.Id)
                    .ToListAsync();

                return new ObservableCollection<Assignment>(assignments);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Aufgaben: " + ex.Message);
                return new ObservableCollection<Assignment>();
            }
        }

        public async Task<ObservableCollection<Assignment>> GetAssignments(User user, AssignmentFilter filter = AssignmentFilter.No)
        {
            try
            {
                IQueryable<Assignment> query = _context.Assignments
                    .Include(a => a.Project)
                    .Include(a => a.User)
                    .Where(a => a.User.Id == user.Id);

                if (filter != AssignmentFilter.No)
                {
                    string priority = filter switch
                    {
                        AssignmentFilter.Priorität_Hoch => "Hoch",
                        AssignmentFilter.Priorität_Mittel => "Mittel",
                        AssignmentFilter.Priorität_Niedrig => "Niedrig",
                        _ => "Hoch"
                    };
                    query = query.Where(a => a.Priority.ToString() == priority);
                }

                var assignments = await query.ToListAsync();
                return new ObservableCollection<Assignment>(assignments);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Aufgaben: " + ex.Message);
                return new ObservableCollection<Assignment>();
            }
        }
    }
}