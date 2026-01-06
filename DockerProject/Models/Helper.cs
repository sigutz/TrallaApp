using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DockerProject.Models;

public static class Helper
{
    public static string GetEnumDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute =
            Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static string GetContrastColor(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor)) return "#000000";
        var cleanHex = hexColor.Replace("#", "");
        if (cleanHex.Length != 6) return "#000000";
        var r = Convert.ToInt32(cleanHex.Substring(0, 2), 16);
        var g = Convert.ToInt32(cleanHex.Substring(2, 2), 16);
        var b = Convert.ToInt32(cleanHex.Substring(4, 2), 16);
        var yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
        return (yiq >= 128) ? "#000000" : "#FFFFFF";
    }

    public static string GetProjectContext(Project project)
    {
        if (project == null) return "Error: Project is null.";

        var sb = new StringBuilder();

        // 1. OVERALL PROJECT CONTEXT
        sb.AppendLine("=== SECTION: OVERALL PROJECT DETAILS ===");
        sb.AppendLine($"Project Title: {project.Title}");
        sb.AppendLine($"Description: {project.Description ?? "No description provided."}");
        sb.AppendLine($"Created Date: {project.CreatedDate:yyyy-MM-dd HH:mm}");
        sb.AppendLine($"Founder: {project.Founder?.UserName ?? "Unknown"}");

        // Fields (Context specific metadata)
        if (project.Fields != null && project.Fields.Any())
        {
            sb.Append("Active Fields/Categories: ");
            sb.AppendLine(string.Join(", ", project.Fields.Select(f => f.Title)));
        }

        sb.AppendLine();

        // 2. MEMBERS
        sb.AppendLine("=== SECTION: MEMBERS ===");
        if (project.Members != null && project.Members.Any())
        {
            foreach (var pm in project.Members)
            {
                var username = pm.Member?.UserName ?? "UnknownUser";
                sb.AppendLine($"- User: {username} | Status: {pm.Status} | Joined: {pm.LastModification:yyyy-MM-dd}");
            }
        }
        else
        {
            sb.AppendLine("No members recorded.");
        }

        sb.AppendLine();

        // 3. TASKS (Detailed Breakdown)
        sb.AppendLine("=== SECTION: TASKS ===");
        if (project.Tasks != null && project.Tasks.Any())
        {
            // Grouping by status helps the AI process structure faster
            var tasksByStatus = project.Tasks.GroupBy(t => t.Status).OrderBy(g => g.Key);

            foreach (var group in tasksByStatus)
            {
                sb.AppendLine($"--- Status: {group.Key} ---");
                foreach (var task in group)
                {
                    sb.AppendLine($"Task Name: {task.Name}");
                    sb.AppendLine($"   ID: {task.Id}"); // Useful if AI needs to cite specific tasks
                    sb.AppendLine($"   Description: {task.Description ?? "None"}");
                    sb.AppendLine($"   Assigned Date: {task.AssignedDate:yyyy-MM-dd}");
                    sb.AppendLine($"   Deadline: {task.DeadLine:yyyy-MM-dd}");

                    if (task.Status == TaskStatusEnum.Done)
                    {
                        sb.AppendLine($"   Done Date: {task.DoneDate:yyyy-MM-dd}");
                    }

                    // Assignees
                    var assignees = task.Users?.Select(u => u.UserName).ToList();
                    sb.AppendLine(
                        $"   Assigned To: {(assignees != null && assignees.Any() ? string.Join(", ", assignees) : "Unassigned")}");

                    // Task Specific Comments (Crucial for "Problems" and "Review" analysis)
                    if (task.Comments != null && task.Comments.Any())
                    {
                        sb.AppendLine("   > Task Comments/Activity:");
                        foreach (var comment in task.Comments.OrderBy(c => c.Date))
                        {
                            var author = comment.Author?.UserName ?? "Unknown";
                            var editedTag = comment.IsEdited ? "(Edited)" : "";
                            sb.AppendLine(
                                $"      - [{comment.Date:yyyy-MM-dd}] {author}: \"{comment.Content}\" {editedTag}");
                        }
                    }

                    sb.AppendLine(); // Spacer between tasks
                }
            }
        }
        else
        {
            sb.AppendLine("No tasks created yet.");
        }

        sb.AppendLine();

        // 4. PROJECT LEVEL DISCUSSIONS
        sb.AppendLine("=== SECTION: GENERAL PROJECT DISCUSSIONS ===");
        // Filter strictly for project-level comments (where TaskParentId is null)
        var generalComments = project.Comments?.Where(c => c.TaskParentId == null).OrderBy(c => c.Date).ToList();

        if (generalComments != null && generalComments.Any())
        {
            foreach (var comment in generalComments)
            {
                var author = comment.Author?.UserName ?? "Unknown";
                sb.AppendLine($"- [{comment.Date:yyyy-MM-dd}] {author}: \"{comment.Content}\"");
            }
        }
        else
        {
            sb.AppendLine("No general project discussions.");
        }

        return sb.ToString();
    }
    
    public static string GetProjectJson(Project project)
    {
        if (project == null) return "{}";

        // We map the EF Entity to a clean anonymous object structure
        // This ensures the AI gets readable dates, string enums, and no circular loops.
        var aiContextPayload = new
        {
            ProjectContext = new
            {
                project.Id,
                Title = project.Title,
                Description = string.IsNullOrWhiteSpace(project.Description) ? "No description provided." : project.Description,
                CreatedDate = project.CreatedDate.ToString("yyyy-MM-dd"),
                Founder = project.Founder?.UserName ?? "Unknown", // Safe navigation
                Fields = project.Fields?.Select(f => f.Title).ToList() ?? new List<string>()
            },
            
            Members = project.Members?.Select(pm => new
            {
                Username = pm.Member?.UserName ?? "Unknown",
                Status = pm.Status.ToString(), // Converts Enum to "Accepted", "Pending", etc.
                LastActive = pm.LastModification.ToString("yyyy-MM-dd")
            }).ToList(),

            // Tasks are crucial for your prompt's logic (ToDo vs Done vs Review)
            Tasks = project.Tasks?.Select(t => new
            {
                t.Id,
                Name = t.Name,
                Description = t.Description,
                Status = t.Status.ToString(), // Converts Enum to "ToDo", "InProgress", "Review", "Done"
                AssignedDate = t.AssignedDate.ToString("yyyy-MM-dd"),
                Deadline = t.DeadLine.ToString("yyyy-MM-dd"),
                // Only include DoneDate if relevant
                DoneDate = t.Status == TaskStatusEnum.Done ? t.DoneDate.ToString("yyyy-MM-dd") : null, 
                
                // Flatten Assignees to simple names
                Assignees = t.Users?.Select(u => u.UserName).ToList(), 
                
                // Task-specific comments (needed for "Problems" and "Review" sections of your prompt)
                Discussions = t.Comments?.Select(c => new
                {
                    Author = c.Author?.UserName ?? "Unknown",
                    Date = c.Date.ToString("yyyy-MM-dd HH:mm"),
                    Content = c.Content,
                    IsEdited = c.IsEdited
                }).OrderBy(c => c.Date).ToList()
            }).OrderBy(t => t.Status).ThenByDescending(t => t.AssignedDate).ToList(),

            // General Project Comments (Global discussions, not attached to specific tasks)
            ProjectGeneralDiscussions = project.Comments?
                .Where(c => c.TaskParentId == null) // Filter out task comments
                .Select(c => new
                {
                    Author = c.Author?.UserName ?? "Unknown",
                    Date = c.Date.ToString("yyyy-MM-dd HH:mm"),
                    Content = c.Content
                })
                .OrderBy(c => c.Date)
                .ToList()
        };

        // Serialize to JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Set to 'false' in production to save AI Tokens
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(aiContextPayload, options);
    }
}