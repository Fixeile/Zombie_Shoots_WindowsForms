using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;

namespace Zombie_Shoots
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        const int playerMargin = 25;
        const int minDistance = 10;
        int x, y;
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int score;
        int scoreLVL;
        int HealthAmmo;
        int pause = 1;
        int level = 1;
        int levelUP = 1;
        int zombieSpeed = 1;
        int collZombie = 3;

        Random rndNum = new Random(Guid.NewGuid().ToByteArray().Sum(x => x));
        
        List<PictureBox> zombiesList = new List<PictureBox>();

        MediaPlayer backgroundMusicPlayer = new MediaPlayer();
        MediaPlayer shootSoundPlayer = new MediaPlayer();
        MediaPlayer dropSoundPlayer = new MediaPlayer();
        MediaPlayer klockSoundPlayer = new MediaPlayer();
        MediaPlayer GameOverSoundPlayer = new MediaPlayer();
        MediaPlayer KritSoundPlayer = new MediaPlayer();
        MediaPlayer LevelUpSiundPlayer = new MediaPlayer();
        MediaPlayer HealthsSoundPlayer = new MediaPlayer();
        MediaPlayer lvlUpSoundPlayer = new MediaPlayer();

        List<string> musicTracks = new List<string>() { "Audio/Night.wav", "Audio/Atomic_Komarova.mp3", @"Audio/TriDnya.mp3", "Audio/Atomic.mp3" };
        int currentTrackIndex = 0;

        public Form1()
        {
            InitializeComponent();

            RestartGame();

            backgroundMusicPlayer.Open(new Uri(musicTracks[currentTrackIndex], UriKind.Relative));
            backgroundMusicPlayer.MediaEnded += (sender, args) =>
            {
                currentTrackIndex++;
                if (currentTrackIndex >= musicTracks.Count)
                {
                    currentTrackIndex = 0;
                }
                backgroundMusicPlayer.Open(new Uri(musicTracks[currentTrackIndex], UriKind.Relative));
                backgroundMusicPlayer.Play();
            };
            backgroundMusicPlayer.Play();

            shootSoundPlayer.Open(new Uri(@"Audio/Bax.wav", UriKind.Relative));
            dropSoundPlayer.Open(new Uri(@"Audio/Drops.wav", UriKind.Relative));
            klockSoundPlayer.Open(new Uri(@"Audio/Knock.mp3", UriKind.Relative));
            GameOverSoundPlayer.Open(new Uri(@"Audio/GameOver.mp3", UriKind.Relative));
            KritSoundPlayer.Open(new Uri(@"Audio/Krit.wav", UriKind.Relative));
            LevelUpSiundPlayer.Open(new Uri(@"Audio/LevelUp.wav", UriKind.Relative));
            HealthsSoundPlayer.Open(new Uri(@"Audio/Healths.mp3", UriKind.Relative));
            lvlUpSoundPlayer.Open(new Uri(@"Audio/LVL_UP.mp3", UriKind.Relative));

        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 100)
            {
                playerHealth = 100;
                healthBar.Value = playerHealth;
            }

            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                GameTimer.Stop();

                backgroundMusicPlayer.Stop();
                GameOverSoundPlayer.Position = TimeSpan.Zero;
                GameOverSoundPlayer.Play();
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;
            txtLevelUp.Text = "Level: " + level;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }
            
            if (goRight == true && player.Left + player.Width < ClientSize.Width)
            {
                player.Left += speed;
            }

            if (goUp == true && player.Top > 40)
            {
                player.Top -= speed;
            }

            if (goDown == true && player.Top + player.Height < ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        dropSoundPlayer.Position = TimeSpan.Zero;
                        dropSoundPlayer.Play();

                        Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "health")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        HealthsSoundPlayer.Position = TimeSpan.Zero;
                        HealthsSoundPlayer.Play();

                        Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        playerHealth += 15;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        KritSoundPlayer.Position = TimeSpan.Zero;
                        KritSoundPlayer.Play();

                        playerHealth -= 1;
                    }

                    if (x.Left > player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }

                    if (x.Left < player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    if (x.Top > player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }

                    if (x.Top < player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }
                }

                foreach (Control j in Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            LevelUpSiundPlayer.Position = TimeSpan.Zero;
                            LevelUpSiundPlayer.Play();

                            score++;
                            scoreLVL++;

                            Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombiesList.Remove((PictureBox)x);
                            MakeZombies();

                            if (scoreLVL == 50 && gameOver == false)
                            {
                                lvlUpSoundPlayer.Position = TimeSpan.Zero;
                                lvlUpSoundPlayer.Play();

                                level += 1;
                                
                                if (levelUP != 4 && gameOver == false)
                                {
                                    levelUP += 1;
                                }

                                if (zombieSpeed < 5)
                                {
                                    zombieSpeed++;
                                }
                                else if (zombieSpeed >= 5)
                                {
                                    zombieSpeed = 5;
                                }
                            }
                            else if (scoreLVL == 120 && gameOver == false)
                            {
                                lvlUpSoundPlayer.Position = TimeSpan.Zero;
                                lvlUpSoundPlayer.Play();

                                level += 1;

                                if (levelUP != 4 && gameOver == false)
                                {
                                    levelUP += 1;
                                }

                                if (zombieSpeed < 5)
                                {
                                    zombieSpeed++;
                                }
                                else if (zombieSpeed >= 5)
                                {
                                    zombieSpeed = 5;
                                }
                            }
                            else if (scoreLVL == 240 && gameOver == false)
                            {
                                lvlUpSoundPlayer.Position = TimeSpan.Zero;
                                lvlUpSoundPlayer.Play();

                                level += 1;

                                if (levelUP != 4 && gameOver == false)
                                {
                                    levelUP += 1;
                                }

                                if (zombieSpeed < 5)
                                {
                                    zombieSpeed++;
                                }
                                else if (zombieSpeed >= 5)
                                {
                                    zombieSpeed = 5;
                                }

                                if (level == 5 && gameOver == false)
                                {
                                    collZombie += 1;
                                }
                                else if (level == 9 && gameOver == false)
                                {
                                    collZombie += 1;
                                }
                                else if (level == 14 && gameOver == false)
                                {
                                    collZombie += 1;
                                }

                                scoreLVL = 0;
                            }
                        }
                    }
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver == true)
            {
                return;
            }

            if (pause == 1)
            {
                if (e.KeyCode == Keys.Left)
                {
                    goLeft = true;
                    facing = "left";
                    player.Image = Properties.Resources.left;
                }

                if (e.KeyCode == Keys.Right)
                {
                    goRight = true;
                    facing = "right";
                    player.Image = Properties.Resources.right;
                }

                if (e.KeyCode == Keys.Up)
                {
                    goUp = true;
                    facing = "up";
                    player.Image = Properties.Resources.up;
                }

                if (e.KeyCode == Keys.Down)
                {
                    goDown = true;
                    facing = "down";
                    player.Image = Properties.Resources.down;
                }
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                if (pause == 1)
                {
                    ammo--;
                    HealthAmmo++;

                    ShootBullet(facing);
                }

                if (ammo < 1)
                {
                    DropAmmo();
                }

                if (HealthAmmo == 48)
                {
                    HealthAmmo = 0;
                    DropHealth();
                }

                if (levelUP == 4 && HealthAmmo == 18)
                {
                    DropHealth();
                    levelUP = 4;
                }
            }

            if (e.KeyCode == Keys.Space && ammo == 0 && gameOver == false)
            {
                klockSoundPlayer.Position = TimeSpan.Zero;
                klockSoundPlayer.Play();
            }

            if (e.KeyCode == Keys.Escape && gameOver == false)
            {
                if (pause == 1)
                {
                    GameTimer.Stop();
                    backgroundMusicPlayer.Pause();
                    pause = 0;
                }
                else if (pause == 0)
                {
                    GameTimer.Start();
                    backgroundMusicPlayer.Play();
                    pause = 1;
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
                backgroundMusicPlayer.Play();
            }
        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();

            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);

            shootSoundPlayer.Position = TimeSpan.Zero;
            shootSoundPlayer.Play();
        }

        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            bool isFarZombie = true;

            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;

            do
            {
                x = rndNum.Next(0, 1800);
                y = rndNum.Next(0, 1000);

                foreach (PictureBox zombies in zombiesList)
                {
                    if (Math.Abs(x - zombie.Left) < minDistance && Math.Abs(y - zombie.Top) < minDistance)
                    {
                        isFarZombie = false;
                        break;
                    }

                    if (Math.Abs(x - player.Left) < playerMargin && Math.Abs(y - player.Top) < playerMargin)
                    {
                        isFarZombie = true;
                    }
                }


            } while (!isFarZombie);

            zombie.Left = x;
            zombie.Top = y;
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);
            Controls.Add(zombie);

            player.BringToFront();
        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = rndNum.Next(10, ClientSize.Width - ammo.Width);
            ammo.Top = rndNum.Next(60, ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            Controls.Add(ammo);

            ammo.BringToFront();
            player.BringToFront();
        }

        private void DropHealth()
        {
            PictureBox health = new PictureBox();
            health.Image = Properties.Resources.Health_Image;
            health.SizeMode = PictureBoxSizeMode.AutoSize;
            health.Left = rndNum.Next(10, ClientSize.Width - health.Width);
            health.Top = rndNum.Next(60, ClientSize.Height - health.Height);
            health.Tag = "health";
            Controls.Add(health);

            health.BringToFront();
            player.BringToFront();
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach (PictureBox i in zombiesList)
            {
                Controls.Remove(i);
            }

            zombiesList.Clear();

            for (int i = 0; i < collZombie; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            scoreLVL = 0;
            ammo = 10;
            level = 1;
            zombieSpeed = 1;

            GameTimer.Start();
        }
    }
}