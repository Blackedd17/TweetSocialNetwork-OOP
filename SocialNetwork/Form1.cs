using Social.Core.Entities;
using Social.Core.Entities.Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Энэхүү Form1 нь social networking platform-ийн үндсэн feed дэлгэц юм.
    /// Энд:
    /// - Story хэсэг
    /// - Feed (постууд)
    /// - Reaction
    /// - Story viewer
    /// зэрэг UI элементүүд ажиллана.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Хэрэглэгчийн мэдээллийг санах in-memory repository.
        /// </summary>
        private InMemoryRepository<User> userRepo = new InMemoryRepository<User>();

        /// <summary>
        /// Постуудын мэдээллийг санах in-memory repository.
        /// </summary>
        private InMemoryRepository<Post> postRepo = new InMemoryRepository<Post>();

        /// <summary>
        /// Story-нуудыг хадгалах жагсаалт.
        /// </summary>
        private List<Story> stories = new List<Story>();

        /// <summary>
        /// Story bubble-уудыг хэвлэх panel.
        /// </summary>
        private FlowLayoutPanel storyPanel;

        /// <summary>
        /// Хэрэглэгчтэй холбоотой үйлдлүүдийг удирдах service.
        /// </summary>
        private UserService userService;

        /// <summary>
        /// Посттой холбоотой үйлдлүүдийг удирдах service.
        /// </summary>
        private PostService postService;

        /// <summary>
        /// Нэвтрэх, бүртгүүлэх логикийг хариуцах service.
        /// </summary>
        private AuthService authService;

        /// <summary>
        /// Одоогоор нэвтэрсэн хэрэглэгч.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Feed доторх пост card-уудыг агуулж харуулах panel.
        /// </summary>
        private FlowLayoutPanel feedPanel;

        /// <summary>
        /// Form1 constructor.
        /// Програм эхлэх үед:
        /// - service-үүд үүснэ
        /// - demo өгөгдөл бэлдэнэ
        /// - UI байгуулна
        /// - stories болон feed ачаална
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            userService = new UserService(userRepo);
            postService = new PostService(postRepo);
            authService = new AuthService(userRepo);

            InitData();
            BuildUI();
            LoadStories();
            LoadFeed();
        }

        /// <summary>
        /// Demo хэрэглэгч, пост, story өгөгдлүүдийг үүсгэнэ.
        /// Хэрэв аль хэдийн байгаа бол дахин үүсгэхгүй.
        /// </summary>
        private void InitData()
        {
            var anar = userService.FindByUsername("anar");
            if (anar == null)
                anar = authService.Register("anar", "Anar Galt", 20, "123");

            var bat = userService.FindByUsername("bat");
            if (bat == null)
                bat = authService.Register("bat", "Bat-Erdene", 21, "123");

            var saraa = userService.FindByUsername("saraa");
            if (saraa == null)
                saraa = authService.Register("saraa", "Saraa", 19, "123");

            currentUser = anar;

            // FOLLOW харилцаа үүсгэж байна
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

            // Demo постууд
            if (postService.GetAllPosts().Count == 0)
            {
                postService.CreatePost(new TextPost(anar.Id, "Hello My Friends 👋"));
                postService.CreatePost(new TextPost(bat.Id, "Ямар уйтгартай өдөр вэээ"));
                postService.CreatePost(new TextPost(saraa.Id, "Маргааш хөвгүүд ууланд гарцгаана"));
                postService.CreatePost(new TextPost(bat.Id, "😄"));
            }

            // Demo story-нууд
            if (stories.Count == 0)
            {
                stories.Add(new Story(currentUser.Id, currentUser.Username, "Story хийж үзье даа"));
                stories.Add(new Story(Guid.NewGuid(), "bat", "Good morning everyone"));
                stories.Add(new Story(Guid.NewGuid(), "saraa", "Coffee time."));
                stories.Add(new Story(Guid.NewGuid(), "temuulen", "Workout workout workout!"));
            }
        }

        /// <summary>
        /// Form-ийн бүх үндсэн UI layout-ийг байгуулна.
        /// Үүнд:
        /// - Header
        /// - Story section
        /// - Feed section
        /// орно.
        /// </summary>
        private void BuildUI()
        {
            this.Text = "SocialX";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ===== HEADER =====
            Panel headerPanel = new Panel();
            headerPanel.Size = new Size(900, 70);
            headerPanel.Location = new Point(0, 0);
            headerPanel.BackColor = Color.White;
            this.Controls.Add(headerPanel);

            Label lblLogo = new Label();
            lblLogo.Text = "SocialX";
            lblLogo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblLogo.ForeColor = Color.FromArgb(24, 119, 242);
            lblLogo.Location = new Point(25, 15);
            lblLogo.AutoSize = true;
            headerPanel.Controls.Add(lblLogo);

            Label lblSub = new Label();
            lblSub.Text = $"Welcome, {currentUser.DisplayName}";
            lblSub.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblSub.ForeColor = Color.Gray;
            lblSub.Location = new Point(30, 48);
            lblSub.AutoSize = true;
            headerPanel.Controls.Add(lblSub);

            // ===== STORIES CONTAINER =====
            Panel storyContainer = new Panel();
            storyContainer.Size = new Size(840, 140);
            storyContainer.Location = new Point(25, 90);
            storyContainer.BackColor = Color.White;
            storyContainer.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(storyContainer);

            Label lblStories = new Label();
            lblStories.Text = "Stories";
            lblStories.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStories.ForeColor = Color.Black;
            lblStories.Location = new Point(15, 10);
            lblStories.AutoSize = true;
            storyContainer.Controls.Add(lblStories);

            storyPanel = new FlowLayoutPanel();
            storyPanel.Size = new Size(810, 90);
            storyPanel.Location = new Point(15, 35);
            storyPanel.AutoScroll = true;
            storyPanel.WrapContents = false;
            storyPanel.BackColor = Color.White;
            storyContainer.Controls.Add(storyPanel);

            // ===== FEED PANEL =====
            feedPanel = new FlowLayoutPanel();
            feedPanel.Name = "feedPanel";
            feedPanel.Size = new Size(840, 450);
            feedPanel.Location = new Point(25, 250);
            feedPanel.AutoScroll = true;
            feedPanel.FlowDirection = FlowDirection.TopDown;
            feedPanel.WrapContents = false;
            feedPanel.BackColor = Color.FromArgb(240, 242, 245);
            this.Controls.Add(feedPanel);
        }

        /// <summary>
        /// Feed panel дээр бүх постуудыг ачаалж харуулна.
        /// Follow хийсэн хэрэглэгчдийн постууд дээгүүр,
        /// бусад постууд доогуур эрэмбэлэгдэнэ.
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
        /// Нэг постыг UI card хэлбэрээр үүсгэнэ.
        /// Card дотор:
        /// - Зохиогчийн нэр
        /// - Username
        /// - Огноо
        /// - Постын агуулга
        /// - Like / Comment / Profile товч
        /// орно.
        /// </summary>
        /// <param name="post">Харуулах пост</param>
        /// <returns>Бэлэн Panel card</returns>
        private Panel CreatePostCard(Post post)
        {
            var author = userService.GetById(post.AuthorId);

            Panel postPanel = new Panel();
            postPanel.Size = new Size(780, 190);
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
            lblTime.Location = new Point(620, 18);
            lblTime.AutoSize = true;
            postPanel.Controls.Add(lblTime);

            Label lblCaption = new Label();
            lblCaption.Text = post.Content;
            lblCaption.Font = new Font("Segoe UI", 11);
            lblCaption.Location = new Point(20, 75);
            lblCaption.Size = new Size(730, 40);
            postPanel.Controls.Add(lblCaption);

            Label lblStats = new Label();
            lblStats.Text = $"❤️ {post.LikeCount}    💬 {post.Comments.Count}";
            lblStats.Font = new Font("Segoe UI Emoji", 10);
            lblStats.ForeColor = Color.DimGray;
            lblStats.Location = new Point(20, 120);
            lblStats.AutoSize = true;
            postPanel.Controls.Add(lblStats);

            // Reaction button
            ReactionButton reactionButton = new ReactionButton();
            reactionButton.Location = new Point(20, 145);
            reactionButton.Size = new Size(130, 32);
            reactionButton.ReactionText = "Like";
            reactionButton.ReactionCount = post.LikeCount;
            reactionButton.Tag = post;
            reactionButton.ReactionChanged += ReactionButton_ReactionChanged;
            postPanel.Controls.Add(reactionButton);

            // Comment button
            Button btnComment = new Button();
            btnComment.Text = "Comment";
            btnComment.Size = new Size(100, 32);
            btnComment.Location = new Point(170, 145);
            btnComment.FlatStyle = FlatStyle.Flat;
            btnComment.BackColor = Color.WhiteSmoke;
            btnComment.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnComment.FlatAppearance.BorderColor = Color.LightGray;
            btnComment.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btnComment.Click += (s, e) =>
            {
                string commentText = Microsoft.VisualBasic.Interaction.InputBox(
                    "Сэтгэгдлээ бичнэ үү:",
                    "Comment",
                    "");

                if (!string.IsNullOrWhiteSpace(commentText))
                {
                    post.Comments.Add(new Comment(currentUser.Id, commentText));
                    LoadFeed();
                }
            };
            postPanel.Controls.Add(btnComment);

            // Profile button
            Button btnProfile = new Button();
            btnProfile.Text = "View Profile";
            btnProfile.Size = new Size(110, 32);
            btnProfile.Location = new Point(290, 145);
            btnProfile.FlatStyle = FlatStyle.Flat;
            btnProfile.BackColor = Color.WhiteSmoke;
            btnProfile.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnProfile.FlatAppearance.BorderColor = Color.LightGray;
            btnProfile.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btnProfile.Click += (s, e) =>
            {
                string name = author != null ? author.DisplayName : "Unknown";
                MessageBox.Show(
                    $"Нэр: {name}\nUsername: @{author?.Username}\nFollowers: {author?.Followers.Count}\nFollowing: {author?.Following.Count}",
                    "User Profile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            postPanel.Controls.Add(btnProfile);

            return postPanel;
        }

        /// <summary>
        /// ReactionButton дээр дарахад reaction picker form-ийг нээж,
        /// сонгосон reaction-ийг тухайн пост дээр хэрэгжүүлнэ.
        /// </summary>
        private void ReactionButton_ReactionChanged(object sender, EventArgs e)
        {
            ReactionButton btn = sender as ReactionButton;
            Post post = btn.Tag as Post;

            if (post == null) return;

            ReactionPickerForm picker = new ReactionPickerForm();

            Point screenPoint = btn.PointToScreen(new Point(0, -70));
            picker.Location = screenPoint;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                string selected = picker.SelectedReaction;

                post.Like(currentUser.Id);
                btn.ReactionCount = post.LikeCount;

                switch (selected)
                {
                    case "Like":
                        btn.ReactionText = "Like";
                        btn.ReactionEmoji = "👍";
                        break;
                    case "Love":
                        btn.ReactionText = "Love";
                        btn.ReactionEmoji = "❤️";
                        break;
                    case "Haha":
                        btn.ReactionText = "Haha";
                        btn.ReactionEmoji = "😂";
                        break;
                    case "Sad":
                        btn.ReactionText = "Sad";
                        btn.ReactionEmoji = "😢";
                        break;
                    case "Angry":
                        btn.ReactionText = "Angry";
                        btn.ReactionEmoji = "😡";
                        break;
                    case "Care":
                        btn.ReactionText = "Care";
                        btn.ReactionEmoji = "🤗";
                        break;
                }

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
        /// Story дээр дарахад StoryViewerForm нээгдэнэ.
        /// Story-г үзсэний дараа viewed төлөвт шилжүүлж,
        /// хүрээний өнгийг саарал болгоно.
        /// </summary>
        private void StoryButton_StoryClicked(object sender, EventArgs e)
        {
            StoryButton btn = sender as StoryButton;
            if (btn == null || btn.StoryData == null) return;

            Story selectedStory = btn.StoryData;

            StoryViewerForm viewer = new StoryViewerForm(selectedStory);
            viewer.ShowDialog();

            selectedStory.IsViewed = true;
            btn.Invalidate();
        }
    }
}