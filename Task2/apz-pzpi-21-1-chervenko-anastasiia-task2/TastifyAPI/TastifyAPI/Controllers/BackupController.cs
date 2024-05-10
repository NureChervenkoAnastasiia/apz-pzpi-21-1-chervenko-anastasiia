using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly string _backupFolderPath;

        public BackupController()
        {
            _backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                if (!Directory.Exists(_backupFolderPath))
                {
                    Directory.CreateDirectory(_backupFolderPath);
                }

                var backupFileName = $"backup_TastifyDB_{DateTime.Now:yyyyMMddHHmmss}";
                var backupFolder = Path.Combine(_backupFolderPath, backupFileName);

                var processInfo = new ProcessStartInfo
                {
                    FileName = "mongodump",
                    Arguments = $"--db TastifyDB --out \"{backupFolder}\"",
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                }

                await Task.Delay(2000);

                backupFolder = Path.Combine(backupFolder, "TastifyDB");
                var bsonFiles = Directory.GetFiles(backupFolder, "*.bson");
                var bsonFile = bsonFiles.FirstOrDefault();

                if (string.IsNullOrEmpty(bsonFile))
                {
                    return StatusCode(500, $"Failed to create backup: Backup file not found.");
                }

                var fileContents = System.IO.File.ReadAllBytes(bsonFile);
                var contentType = "application/octet-stream";

                return File(fileContents, contentType, Path.GetFileName(bsonFile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to create backup: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public IActionResult GetBackupList()
        {
            try
            {
                if (!Directory.Exists(_backupFolderPath))
                {
                    return Ok(new string[] { });
                }

                var backupFolders = Directory.GetDirectories(_backupFolderPath)
                    .Select(Path.GetFileName)
                    .OrderByDescending(folder => folder);

                return Ok(backupFolders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get backup list: {ex.Message}");
            }
        }

        [HttpPost("restore")]
        public async Task<IActionResult> RestoreBackup([FromForm] string backupFileName)
        {
            try
            {
                var backupFilePath = Path.Combine(_backupFolderPath, backupFileName);

                var processInfo = new ProcessStartInfo
                {
                    FileName = "mongorestore",
                    Arguments = $"--drop --db TastifyDB \"{backupFilePath}\"",
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                }

                return Ok("Database restored successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to restore database: {ex.Message}");
            }
        }
    }
}

