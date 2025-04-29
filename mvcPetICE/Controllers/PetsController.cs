using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcPetICE.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace mvcPetICE.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string containerName = "images";
        private readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=petsice;AccountKey=HLHcCM+os0Lic0ro0S5t/QBSanXJKG71Ifhay5LbTgdQ31iGcQgeD1ubezBzFr1KgcW1d38IHCb2+AStq0Gvug==;EndpointSuffix=core.windows.net";
        
        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Pets = await _context.Pets.ToListAsync();
            return View(Pets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var Pets = await _context.Pets
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Pets == null)
                return NotFound();

            return View(Pets);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(Pets Pets)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Pets);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Pets);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Pets = await _context.Pets.FindAsync(id);

            if (Pets == null)
            {
                return NotFound();
            }
            return View(Pets);
        }

        private bool PetsExists(int id)
        {
            return _context.Pets.Any(c => c.Id == id);
        }
        [HttpPost]

        public async Task<IActionResult> Edit(int id, Pets Pets)
        {
            if (id != Pets.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Pets);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetsExists(Pets.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(Pets);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Pets = await _context.Pets.FirstOrDefaultAsync(c => c.PetsId == id);

            if (Pets == null)
            {
                return NotFound();
            }
            return View(Pets);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                //Upload the file to Blob Storage
                await UploadFileToBlobStorageAsync(uploadedFile);
            }
            //Redirect back to the Index view to refresh the file list
            return RedirectToAction("Index");
        }

        public IActionResult ViewFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return NotFound("File not found"); //Return 404 if file URL is not provided
            }
            ViewBag.FileUrl = fileUrl; //Pass the file URL to the view
            return View();
        }
        private async Task<List<string>> FetchImageUrlsAsync()
        {
            var imageUrls = new List<string>();
            var containerClient = new BlobContainerClient(connectionString, containerName);

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                imageUrls.Add(blobClient.Uri.ToString());
            }

            return imageUrls;
        }
        private async Task UploadFileToBlobStorageAsync(IFormFile uploadedFile)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob); //Ensure the container exists

            //Create a BlobClient for the uploaded file
            var blobClient = containerClient.GetBlobClient(uploadedFile.FileName);
            //Upload the file stream asynchronously
            using (var stream = uploadedFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }
        }
    }
    }
