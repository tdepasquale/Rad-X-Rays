# Rad X-Rays

Rad X-Rays is a PWA designed to help x-ray techs excel at their job.

# Technique Charts

A technique chart is a list of various x-ray techniques that a tech might use. 
Registered users can build, save and share technique charts.
The charts are printer friendly and will remove excess page elements when a user prints one.
Techniques and be easily organized by dragging and dropping.
Drag and drop functionality was achieved using react-beautiful-dnd.

Our technique chart tool is great for students, technologists and managers. 
Use it to create a chart for each room in your facility as required by law. 
Technologists and students can use it to keep track of ideal techniques in each room they work in. 
One of the hardest parts of being a student or starting a new job is getting used to the quirks of each x-ray machine, which includes techniques. 
We’ve all experienced an x-ray tube that shoots “hot” or “cold” and here you can keep track of everything you need.

Drag and drop to put the chart in the order of your choosing. 
Easily adjust with the click of a button or the tap of your finger on mobile. 
Delete or add rows for specific body parts/positions you want in your chart. 
Finally, name your chart so you can find it easily for reference. No more relying on google for suggested techniques. 
Keep track of the ones you know work! Great for differentiating between adult and pediatric techniques as well as the asthenic, sthenic and hypersthenic patient!

# Blog Posts and Positioning Guides

Registered users with the correct role permissions can create and submit blogs as well as positioning guides.
Posts are created by adding various sections including titles, paragraphs, images, and list items.
Sections can be easily rearranged by dragging and dropping.
Drag and drop functionality was achieved using react-beautiful-dnd.
Once a blog / guide is submitted a user with the correct role permissions can review the post.
They can then either post it or give feedback and send it back for revision.
Blog / guide images are uploaded to an Amazon S3 bucket.
React-Linkify was used to appropriately render links in blogs / guides.

# Frontend

The frontend was built using Create-React-App, HTML, CSS, Typescript / Javascript, and Semantic UI.
Toast notifications were implemented using React-Toastify.
The modals and icons are from Semantic UI.
Animations use React-Transition-Group.
Forms were built with Final-Form and Revalidate.
React-Helmet was used to make changes to the document head. For example, setting a different title for each page.

# Backend

The backend was built using .Net Core in C#.
Security was added using NWebsec.
The database is mySql built with Entity Framework.
Auth is achieved using JWT and the JwtBearer library.
Actions were written using the CQRS Mediator pattern and the MediatR library.

# Deployment and Hosting

The app was deployed from vscode using an extension called DeployReloaded.
It is currently hosted in a linux droplet on Digital Ocean using a modified LAMP stack.
