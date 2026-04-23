using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;
using Social.Infrastructure.Data;
using Social.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Social networking platform-ийн үндсэн feed form.
    /// 
    /// Энэ form нь:
    /// - SQLite database-тэй холбогдоно
    /// - Хэрэглэгч, пост, comment, reaction өгөгдөлтэй ажиллана
    /// - Story, create post, feed зэрэг UI хэсгүүдийг харуулна
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// SQLite өгөгдлийн сангийн context.
        /// </summary>
        private readonly SqliteDbContext db = new SqliteDbContext();

        /// <summary>
        /// User өгөгдөл хадгалах repository.
        /// </summary>
        private IRepository<User> userRepo;

        /// <summary>
        /// Post өгөгдөл хадгалах repository.
        /// </summary>
        private IPostRepository postRepo;

        /// <summary>
        /// Comment өгөгдөл хадгалах repository.
        /// </summary>
        private ICommentRepository commentRepo;

        /// <summary>
        /// Reaction өгөгдөл хадгалах repository.
        /// </summary>
        private IReactionRepository reactionRepo;

        /// <summary>
        /// Story жагсаалт.
        /// </summary>
        private readonly List<Story> stories = new List<Story>();

        /// <summary>
        /// Story bubble-уудыг харуулах panel.
        /// </summary>
        private FlowLayoutPanel storyPanel;

        /// <summary>
        /// Feed card-уудыг харуулах panel.
        /// </summary>
        private FlowLayoutPanel feedPanel;

        /// <summary>
        /// Шинэ пост бичих textbox.
        /// </summary>
        private TextBox txtCreatePost;

        /// <summary>
        /// Пост үүсгэх button.
        /// </summary>
        private Button btnCreatePost;

        /// <summary>
        /// User-тэй холбоотой service.
        /// </summary>
        private UserService userService;

        /// <summary>
        /// Post-той холбоотой service.
        /// </summary>
        private PostService postService;

        /// <summary>
        /// Login/Register service.
        /// </summary>
        private AuthService authService;

        /// <summary>
        /// Одоогоор нэвтэрсэн хэрэглэгч.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Дэмжигдэх reaction төрлүүд.
        /// </summary>
        private readonly string[] reactionTypes = new string[]
        {
            "Like",
            "Love",
            "Haha",
            "Sad",
            "Angry",
            "Care"
        };

        /// <summary>
        /// Form1-ийн constructor.
        /// 
        /// Энэ үед:
        /// - Database initialize хийнэ
        /// - Repository, service-үүдийг үүсгэнэ
        /// - Demo өгөгдөл бэлдэнэ
        /// - UI байгуулна
        /// - Stories болон feed-ийг ачаална
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            db.Initialize();

            userRepo = new SQLiteUserRepository(db);
            postRepo = new SQLitePostRepository(db);
            commentRepo = new SQLiteCommentRepository(db);
            reactionRepo = new SQLiteReactionRepository(db);

            userService = new UserService(userRepo);
            postService = new PostService(postRepo);
            authService = new AuthService(userRepo);

            InitData();
            BuildUI();
            LoadStories();
            LoadFeed();
        }

        /// <summary>
        /// Demo хэрэглэгч, пост, story өгөгдөл үүсгэнэ.
        /// Хэрэв аль хэдийн байгаа бол дахин нэмэхгүй.
        /// </summary>
        private void InitData()
        {
            var anar = userService.FindByUsername("anar");
            if (anar == null)
            {
                anar = authService.Register("anar", "Anar Galt", 20, "123");
            }

            var bat = userService.FindByUsername("bat");
            if (bat == null)
            {
                bat = authService.Register("bat", "Bat-Erdene", 21, "123");
            }

            var saraa = userService.FindByUsername("saraa");
            if (saraa == null)
            {
                saraa = authService.Register("saraa", "Saraa", 19, "123");
            }

            currentUser = anar;

            if (!currentUser.Following.Contains(bat.Id))
            {
                currentUser.Follow(bat.Id);
                bat.Followers.Add(currentUser.Id);
            }

            if (!currentUser.Following.Contains(saraa.Id))
            {
                currentUser.Follow(saraa.Id);
                saraa.Followers.Add(currentUser.Id);
            }

            if (postService.GetAllPosts().Count == 0)
            {
                postService.CreatePost(new TextPost(anar.Id, "Hello My Friends 👋"));
                postService.CreatePost(new TextPost(bat.Id, "Ямар уйтгартай өдөр вэээ"));
                postService.CreatePost(new TextPost(saraa.Id, "Маргааш хөвгүүд ууланд гарцгаана"));
                postService.CreatePost(new TextPost(bat.Id, "😄"));
            }

            if (stories.Count == 0)
            {
                stories.Add(new Story(currentUser.Id, currentUser.Username, "Story хийж үзье даа"));
                stories.Add(new Story(Guid.NewGuid(), "bat", "Good morning everyone"));
                stories.Add(new Story(Guid.NewGuid(), "saraa", "Coffee time."));
                stories.Add(new Story(Guid.NewGuid(), "temuulen", "Workout workout workout!"));
            }
        }

        /// <summary>
        /// Form-ийн үндсэн UI layout-ийг байгуулна.
        /// </summary>
        private void BuildUI()
        {
            Text = "SocialX";
            Size = new Size(940, 820);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 242, 245);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            BuildHeader();
            BuildCreatePostSection();
            BuildStorySection();
            BuildFeedSection();
        }

        /// <summary>
        /// Header хэсгийг байгуулна.
        /// </summary>
        private void BuildHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.Size = new Size(940, 72);
            headerPanel.Location = new Point(0, 0);
            headerPanel.BackColor = Color.White;
            Controls.Add(headerPanel);

            Label lblLogo = new Label();
            lblLogo.Text = "SocialX";
            lblLogo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblLogo.ForeColor = Color.FromArgb(24, 119, 242);
            lblLogo.Location = new Point(25, 14);
            lblLogo.AutoSize = true;
            headerPanel.Controls.Add(lblLogo);

            Label lblSub = new Label();
            lblSub.Text = "Welcome, " + currentUser.DisplayName;
            lblSub.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblSub.ForeColor = Color.Gray;
            lblSub.Location = new Point(30, 48);
            lblSub.AutoSize = true;
            headerPanel.Controls.Add(lblSub);
        }

        /// <summary>
        /// Шинэ пост бичих хэсгийг байгуулна.
        /// </summary>
        private void BuildCreatePostSection()
        {
            Panel createPostPanel = new Panel();
            createPostPanel.Size = new Size(880, 90);
            createPostPanel.Location = new Point(25, 85);
            createPostPanel.BackColor = Color.White;
            createPostPanel.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(createPostPanel);

            Label lblCreate = new Label();
            lblCreate.Text = "Create Post";
            lblCreate.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCreate.Location = new Point(15, 10);
            lblCreate.AutoSize = true;
            createPostPanel.Controls.Add(lblCreate);

            txtCreatePost = new TextBox();
            txtCreatePost.Location = new Point(18, 40);
            txtCreatePost.Size = new Size(720, 26);
            txtCreatePost.Font = new Font("Segoe UI", 10);
            createPostPanel.Controls.Add(txtCreatePost);

            btnCreatePost = new Button();
            btnCreatePost.Text = "Post";
            btnCreatePost.Location = new Point(755, 38);
            btnCreatePost.Size = new Size(100, 30);
            btnCreatePost.FlatStyle = FlatStyle.Flat;
            btnCreatePost.BackColor = Color.FromArgb(24, 119, 242);
            btnCreatePost.ForeColor = Color.White;
            btnCreatePost.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnCreatePost.Click += BtnCreatePost_Click;
            createPostPanel.Controls.Add(btnCreatePost);
        }

        /// <summary>
        /// Story хэсгийг байгуулна.
        /// </summary>
        private void BuildStorySection()
        {
            Panel storyContainer = new Panel();
            storyContainer.Size = new Size(880, 145);
            storyContainer.Location = new Point(25, 190);
            storyContainer.BackColor = Color.White;
            storyContainer.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(storyContainer);

            Label lblStories = new Label();
            lblStories.Text = "Stories";
            lblStories.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStories.Location = new Point(15, 10);
            lblStories.AutoSize = true;
            storyContainer.Controls.Add(lblStories);

            storyPanel = new FlowLayoutPanel();
            storyPanel.Size = new Size(845, 95);
            storyPanel.Location = new Point(15, 35);
            storyPanel.AutoScroll = true;
            storyPanel.WrapContents = false;
            storyPanel.BackColor = Color.White;
            storyContainer.Controls.Add(storyPanel);
        }

        /// <summary>
        /// Feed хэсгийг байгуулна.
        /// </summary>
        private void BuildFeedSection()
        {
            feedPanel = new FlowLayoutPanel();
            feedPanel.Name = "feedPanel";
            feedPanel.Size = new Size(880, 430);
            feedPanel.Location = new Point(25, 350);
            feedPanel.AutoScroll = true;
            feedPanel.FlowDirection = FlowDirection.TopDown;
            feedPanel.WrapContents = false;
            feedPanel.BackColor = Color.FromArgb(240, 242, 245);
            Controls.Add(feedPanel);
        }

        /// <summary>
        /// Post button дээр дарахад шинэ пост үүсгэнэ.
        /// </summary>
        private void BtnCreatePost_Click(object sender, EventArgs e)
        {
            string text = txtCreatePost.Text.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(
                    "Постын текст хоосон байж болохгүй.",
                    "Анхаар",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            postService.CreatePost(new TextPost(currentUser.Id, text));
            txtCreatePost.Clear();
            LoadFeed();
        }

        /// <summary>
        /// Feed panel дээр бүх постуудыг ачаалж харуулна.
        /// Follow хийсэн хэрэглэгчдийн постуудыг дээгүүр эрэмбэлнэ.
        /// </summary>
        private void LoadFeed()
        {
            feedPanel.Controls.Clear();

            var posts = postService.GetAllPosts()
                .OrderByDescending(p => currentUser.Following.Contains(p.AuthorId))
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            foreach (var post in posts)
            {
                feedPanel.Controls.Add(CreatePostCard(post));
            }
        }

        /// <summary>
        /// Нэг постыг card хэлбэрийн panel болгон үүсгэнэ.
        /// </summary>
        /// <param name="post">Харуулах пост</param>
        /// <returns>Бэлэн post card panel</returns>
        private Panel CreatePostCard(Post post)
        {
            var author = userService.GetById(post.AuthorId);
            int totalReactionCount = GetTotalReactionCount(post.Id);
            int commentCount = GetCommentCount(post.Id);
            string currentReaction = reactionRepo.GetUserReaction(post.Id, currentUser.Id);

            Panel postPanel = new Panel();
            postPanel.Size = new Size(830, 215);
            postPanel.BackColor = Color.White;
            postPanel.BorderStyle = BorderStyle.FixedSingle;
            postPanel.Margin = new Padding(10);

            Label lblUser = new Label();
            lblUser.Text = author != null ? author.DisplayName : "Unknown";
            lblUser.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblUser.Location = new Point(20, 15);
            lblUser.AutoSize = true;
            postPanel.Controls.Add(lblUser);

            Label lblUsername = new Label();
            lblUsername.Text = author != null ? "@" + author.Username : "@unknown";
            lblUsername.Font = new Font("Segoe UI", 9);
            lblUsername.ForeColor = Color.Gray;
            lblUsername.Location = new Point(22, 40);
            lblUsername.AutoSize = true;
            postPanel.Controls.Add(lblUsername);

            Label lblTime = new Label();
            lblTime.Text = post.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            lblTime.Font = new Font("Segoe UI", 8);
            lblTime.ForeColor = Color.Gray;
            lblTime.Location = new Point(650, 18);
            lblTime.AutoSize = true;
            postPanel.Controls.Add(lblTime);

            Label lblCaption = new Label();
            lblCaption.Text = post.Content;
            lblCaption.Font = new Font("Segoe UI", 11);
            lblCaption.Location = new Point(20, 75);
            lblCaption.MaximumSize = new Size(770, 0);
            lblCaption.AutoSize = true;
            postPanel.Controls.Add(lblCaption);

            Label lblStats = new Label();
            lblStats.Text = "Reactions: " + totalReactionCount + "    Comments: " + commentCount;
            lblStats.Font = new Font("Segoe UI", 10);
            lblStats.ForeColor = Color.DimGray;
            lblStats.Location = new Point(20, 125);
            lblStats.AutoSize = true;
            postPanel.Controls.Add(lblStats);

            ReactionButton reactionButton = new ReactionButton();
            reactionButton.Location = new Point(20, 160);
            reactionButton.Size = new Size(140, 32);
            reactionButton.Tag = post;
            reactionButton.ReactionCount = totalReactionCount;
            ApplyReactionAppearance(reactionButton, currentReaction);
            reactionButton.ReactionChanged += ReactionButton_ReactionChanged;
            postPanel.Controls.Add(reactionButton);

            Button btnComment = new Button();
            btnComment.Text = "Comment";
            btnComment.Size = new Size(100, 32);
            btnComment.Location = new Point(180, 160);
            btnComment.FlatStyle = FlatStyle.Flat;
            btnComment.BackColor = Color.WhiteSmoke;
            btnComment.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnComment.FlatAppearance.BorderColor = Color.LightGray;
            btnComment.Click += (s, e) =>
            {
                string commentText = Microsoft.VisualBasic.Interaction.InputBox(
                    "Сэтгэгдлээ бичнэ үү:",
                    "Comment",
                    "");

                if (!string.IsNullOrWhiteSpace(commentText))
                {
                    var comment = new Comment(post.Id, currentUser.Id, commentText);
                    commentRepo.Add(comment);
                    LoadFeed();
                }
            };
            postPanel.Controls.Add(btnComment);

            Button btnViewComments = new Button();
            btnViewComments.Text = "View Comments";
            btnViewComments.Size = new Size(120, 32);
            btnViewComments.Location = new Point(300, 160);
            btnViewComments.FlatStyle = FlatStyle.Flat;
            btnViewComments.BackColor = Color.WhiteSmoke;
            btnViewComments.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnViewComments.FlatAppearance.BorderColor = Color.LightGray;
            btnViewComments.Click += (s, e) =>
            {
                ShowComments(post);
            };
            postPanel.Controls.Add(btnViewComments);

            Button btnProfile = new Button();
            btnProfile.Text = "View Profile";
            btnProfile.Size = new Size(110, 32);
            btnProfile.Location = new Point(440, 160);
            btnProfile.FlatStyle = FlatStyle.Flat;
            btnProfile.BackColor = Color.WhiteSmoke;
            btnProfile.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnProfile.FlatAppearance.BorderColor = Color.LightGray;
            btnProfile.Click += (s, e) =>
            {
                string name = author != null ? author.DisplayName : "Unknown";
                MessageBox.Show(
                    "Нэр: " + name +
                    "\nUsername: @" + (author != null ? author.Username : "unknown") +
                    "\nFollowers: " + (author != null ? author.Followers.Count.ToString() : "0") +
                    "\nFollowing: " + (author != null ? author.Following.Count.ToString() : "0"),
                    "User Profile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            postPanel.Controls.Add(btnProfile);

            return postPanel;
        }

        /// <summary>
        /// Reaction button дархад reaction picker нээж,
        /// сонгосон reaction-ийг database-д хадгална.
        /// </summary>
        private void ReactionButton_ReactionChanged(object sender, EventArgs e)
        {
            ReactionButton btn = sender as ReactionButton;
            Post post = btn != null ? btn.Tag as Post : null;

            if (post == null)
            {
                return;
            }

            ReactionPickerForm picker = new ReactionPickerForm();

            Point screenPoint = btn.PointToScreen(new Point(0, -70));
            picker.Location = screenPoint;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                string selected = picker.SelectedReaction;
                reactionRepo.ToggleReaction(post.Id, currentUser.Id, selected);
                LoadFeed();
            }
        }

        /// <summary>
        /// Story жагсаалтыг storyPanel дээр bubble хэлбэрээр ачаална.
        /// </summary>
        private void LoadStories()
        {
            storyPanel.Controls.Clear();

            foreach (var story in stories)
            {
                StoryButton storyButton = new StoryButton();
                storyButton.StoryData = story;
                storyButton.Margin = new Padding(10);
                storyButton.StoryClicked += StoryButton_StoryClicked;
                storyPanel.Controls.Add(storyButton);
            }
        }

        /// <summary>
        /// Story дээр дарахад StoryViewerForm нээнэ.
        /// Үзсэний дараа viewed төлөвт оруулна.
        /// </summary>
        private void StoryButton_StoryClicked(object sender, EventArgs e)
        {
            StoryButton btn = sender as StoryButton;
            if (btn == null || btn.StoryData == null)
            {
                return;
            }

            Story selectedStory = btn.StoryData;
            StoryViewerForm viewer = new StoryViewerForm(selectedStory);
            viewer.ShowDialog();

            selectedStory.IsViewed = true;
            btn.Invalidate();
        }

        /// <summary>
        /// Тухайн постын нийт reaction тоог бодож буцаана.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Нийт reaction тоо</returns>
        private int GetTotalReactionCount(Guid postId)
        {
            int total = 0;

            foreach (string type in reactionTypes)
            {
                total += reactionRepo.GetCount(postId, type);
            }

            return total;
        }

        /// <summary>
        /// Тухайн постын comment тоог буцаана.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Comment тоо</returns>
        private int GetCommentCount(Guid postId)
        {
            return commentRepo.GetByPostId(postId).Count;
        }

        /// <summary>
        /// Хэрэглэгчийн өгсөн reaction төрөлд тааруулж button-ийн
        /// текст болон emoji-г тохируулна.
        /// </summary>
        /// <param name="button">Reaction button</param>
        /// <param name="reactionType">Reaction төрөл</param>
        private void ApplyReactionAppearance(ReactionButton button, string reactionType)
        {
            string type = string.IsNullOrWhiteSpace(reactionType) ? "Like" : reactionType;

            switch (type)
            {
                case "Love":
                    button.ReactionText = "Love";
                    button.ReactionEmoji = "❤️";
                    break;

                case "Haha":
                    button.ReactionText = "Haha";
                    button.ReactionEmoji = "😂";
                    break;

                case "Sad":
                    button.ReactionText = "Sad";
                    button.ReactionEmoji = "😢";
                    break;

                case "Angry":
                    button.ReactionText = "Angry";
                    button.ReactionEmoji = "😡";
                    break;

                case "Care":
                    button.ReactionText = "Care";
                    button.ReactionEmoji = "🤗";
                    break;

                default:
                    button.ReactionText = "Like";
                    button.ReactionEmoji = "👍";
                    break;
            }
        }

        /// <summary>
        /// Тухайн постын бүх comment-уудыг MessageBox дээр харуулна.
        /// </summary>
        /// <param name="post">Сонгосон пост</param>
        private void ShowComments(Post post)
        {
            var comments = commentRepo.GetByPostId(post.Id);

            if (comments.Count == 0)
            {
                MessageBox.Show(
                    "Одоогоор comment алга байна.",
                    "Comments",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            StringBuilder builder = new StringBuilder();

            foreach (var comment in comments)
            {
                var user = userService.GetById(comment.UserId);
                string displayName = user != null ? user.DisplayName : "Unknown";

                builder.AppendLine(displayName + ": " + comment.Text);
                builder.AppendLine();
            }

            MessageBox.Show(
                builder.ToString(),
                "Comments",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}