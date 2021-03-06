﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DI = Microsoft.DirectX.DirectInput;

using DXAV = Microsoft.DirectX.AudioVideoPlayback;
using DSND = Microsoft.DirectX.DirectSound;

using System.Timers;
using System.Threading;

namespace AccelerationDirectX
{
    public partial class Form1 : Form
    {
        private Device device = null;

        public const int ScreenWidth = 1024;
        public const int ScreenHeight = 768;

        private bool fullscreen = false;

        private VertexBuffer vb = null;
        private VertexBuffer vbTerrain = null;
        private IndexBuffer ib = null;
        private IndexBuffer ib2 = null;

        private DXAV.Audio bMusic;

        private DSND.Device soundDevice = null;
        private DSND.SecondaryBuffer sound1 = null;
        private DSND.SecondaryBuffer sound2 = null;


        private Microsoft.DirectX.Direct3D.Font infoFont;

        private Mesh tor = null;
        private Mesh glob = null;
        private Mesh pil = null;

        private Texture texI = null;

        private float angleTor = 0.0f;
        private float angleTorStep = 0.002f;

        private float pyrUpDown = 0.0f;
        private float pyrUpDownStep = 0.005f;

        private float angleGlob = 0.0f;
        private float angleGlobStep = 0.02f;

        private float moveStep = 0.25f;

        private Camera myCamera = null;

        private Microsoft.DirectX.DirectInput.Device keyb;

        protected System.Timers.Timer myTimer;

        private int TIMER_INTERVAL = 20;

        // DOUBLE PYRAMID 0-5
        private static readonly short[] indices = {
            1,4,2,
            0,4,1,
            3,4,0,
            2,4,3,
            1,2,5,
            0,1,5,
            3,0,5,
            2,3,5
        };

        // PYRAMID 6-10
        private static readonly short[] indices2 = {
           7,10,8,
           6,10,7,
           9,10,6,
           8,10,9,
           6,7,8,
           8,9,6
        };

        private MatrixStack mStack = new MatrixStack();

        private WiiController wii;

        private bool showInfo = true;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        public void InitializeTimer()
        {
            myTimer = new System.Timers.Timer();

            myTimer.Elapsed += new ElapsedEventHandler(TimerEventProcessor);
            myTimer.Interval = TIMER_INTERVAL;
            myTimer.Start();
        }

        public void InitializeWii()
        {
            wii = new WiiController();
        }

        public void InitializeKeyboard()
        {
            keyb = new DI.Device(DI.SystemGuid.Keyboard);
            keyb.SetCooperativeLevel(this, DI.CooperativeLevelFlags.Background | DI.CooperativeLevelFlags.NonExclusive);
            keyb.Acquire();
        }

        public void InitializeAudio()
        {
            bMusic = new DXAV.Audio("m.wav");
            bMusic.Ending += new EventHandler(bMusic_Ending);
            bMusic.Play();

            soundDevice = new DSND.Device();
            soundDevice.SetCooperativeLevel(this, DSND.CooperativeLevel.Normal);
            sound1 = new DSND.SecondaryBuffer(@"..\..\m1.wav", soundDevice);
            sound2 = new DSND.SecondaryBuffer(@"..\..\m2.wav", soundDevice);
        }

        void bMusic_Ending(object sender, EventArgs e)
        {
            bMusic.SeekCurrentPosition(0, DXAV.SeekPositionFlags.AbsolutePositioning);
        }


        public void InitializeGraphicsAndCamera()
        {
            // Set presentation parameters
            PresentParameters presentParams = new PresentParameters();

            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            /*
            // Start up full screen
            Format current = Manager.Adapters[0].CurrentDisplayMode.Format;

            if (Manager.CheckDeviceType(0, DeviceType.Hardware, current, current, false))
            {
                // Perfect, this is valid
                presentParams.Windowed = false;
                presentParams.BackBufferFormat = current;
                presentParams.BackBufferCount = 1;
                presentParams.BackBufferWidth = ScreenWidth;
                presentParams.BackBufferHeight = ScreenHeight;
                Cursor.Hide();
                fullscreen = true;
            }
            else
            {
                presentParams.Windowed = true;
                fullscreen = false;
            }
            */

            // Depth Buffer
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;

            // Create device
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            // Create vertex buffer and index buffers
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 11, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);
            vb.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vb, null);

            ib = new IndexBuffer(typeof(short), indices.Length, device, Usage.WriteOnly, Pool.Default);
            ib.Created += new EventHandler(this.OnIndexBufferCreate);
            OnIndexBufferCreate(ib, null);

            ib2 = new IndexBuffer(typeof(short), indices2.Length, device, Usage.WriteOnly, Pool.Default);
            ib2.Created += new EventHandler(this.OnIndexBufferCreate);
            OnIndexBufferCreate(ib2, null);

            tor = Mesh.Torus(device, 0.2f, 0.8f, 72, 72);
            glob = Mesh.Sphere(device, 0.2f, 72, 72);
            //pil = Mesh.Box(device, 0.4f, 0.4f, 0.4f);
            //pil = Mesh.Cylinder(device, 0.4f, 0.3f, 1.0f, 72, 72);
            pil = Mesh.Sphere(device, 0.5f, 72, 72);

            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 800, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);
            vbTerrain.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vbTerrain, null);

            texI = new Texture(device, new Bitmap("i.jpg"), 0, Pool.Managed);

            infoFont = new Microsoft.DirectX.Direct3D.Font( device, new System.Drawing.Font("Arial", 12.0f, FontStyle.Regular));

            mStack = new MatrixStack();

            myCamera = new Camera(new Vector3(0.0f, 5.0f, -14.0f), new Vector3(0.0f, 5.0f, -13.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f,0.0f,1.0f), new Vector3(0.0f,0.0f,1.0f));
        }

        private void OnIndexBufferCreate(object sender, EventArgs e)
        {
            IndexBuffer buffer = (IndexBuffer)sender;

            if(buffer == ib)
                buffer.SetData(indices, 0, LockFlags.None);

            if(buffer == ib2)
                buffer.SetData(indices2, 0, LockFlags.None);
        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer buffer = (VertexBuffer)sender;

            if (buffer == vb)
            {
                CustomVertex.PositionNormalColored[] verts = new CustomVertex.PositionNormalColored[11];
                // DOUBLE PYRAMID
                verts[0] = new CustomVertex.PositionNormalColored(new Vector3(-1.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Purple.ToArgb());
                verts[1] = new CustomVertex.PositionNormalColored(new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Yellow.ToArgb());
                verts[2] = new CustomVertex.PositionNormalColored(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Red.ToArgb());
                verts[3] = new CustomVertex.PositionNormalColored(new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Blue.ToArgb());

                verts[4] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, 2.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), Color.Green.ToArgb());
                verts[5] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, -2.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), Color.WhiteSmoke.ToArgb());

                // PYRAMID
                verts[6] = new CustomVertex.PositionNormalColored(new Vector3(-10.0f, 0.0f, 10.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Gold.ToArgb());
                verts[7] = new CustomVertex.PositionNormalColored(new Vector3(-10.0f, 0.0f, -10.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Gold.ToArgb());                
                verts[8] = new CustomVertex.PositionNormalColored(new Vector3(10.0f, 0.0f, -10.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Gold.ToArgb());
                verts[9] = new CustomVertex.PositionNormalColored(new Vector3(10.0f, 0.0f, 10.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.Gold.ToArgb());
                verts[10] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, 12.0f, 0.0f), new Vector3(0.0f, 1.0f, -1.0f), Color.WhiteSmoke.ToArgb());

                buffer.SetData(verts, 0, LockFlags.None);
            }

            if (buffer == vbTerrain)
            {
                CustomVertex.PositionNormalColored[] vertsTerrain = new CustomVertex.PositionNormalColored[800];
                for (int i = -100; i < 100; i++)
                {
                    int index = i + 100;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((-100.0f), 0.0f, (float)i), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(100.0f, 0.0f, (float)i), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 0.0f, -100.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 0.0f, 100.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                }
                buffer.SetData(vertsTerrain, 0, LockFlags.None);
            }

        }

        private void SetupCamera()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1.0f, 10000.0f);
            //device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, -15.0f), new Vector3(0,0,0), new Vector3(0, 1, 0));
            device.Transform.View = myCamera.LookLeftHanded();

            // Setting back face culling
            device.RenderState.CullMode = Cull.None;

            // Setting lights

            device.RenderState.Lighting = true;

            //device.RenderState.Ambient = Color.Bisque;
            //device.Lights[0].Ambient = Color.FromArgb(20,20,20);
            device.Lights[0].Type = LightType.Directional;
            //device.Lights[0].Diffuse = Color.DarkBlue;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0.0f, -0.5f, 1.0f);
            device.Lights[0].Enabled = true;

            Color lightDiffColor = Color.White;
            if (wii != null)
                if (wii.Connected)
                    lightDiffColor = Color.Red;

            device.Lights[1].Type = LightType.Directional;
            device.Lights[1].Diffuse = lightDiffColor;
            //device.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            device.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            device.Lights[1].Enabled = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Color backColor = Color.CornflowerBlue;

            if (wii != null)
                if (wii.Connected)
                    backColor = Color.DarkKhaki;

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backColor, 1.0f, 0);

            SetupCamera();

            device.BeginScene();
            
            //mStack.LoadMatrix(device.Transform.View);

            mStack.Push();
            {
                mStack.MultiplyMatrixLocal(
                        Matrix.Identity
                        );
                device.Transform.World = mStack.Top;

                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.SetStreamSource(0, vbTerrain, 0);
                device.DrawPrimitives(PrimitiveType.LineList, 0, 400);
            }
            mStack.Pop();

            mStack.Push();
            {
                mStack.Push();
                {
                    //DOUBLE PYRAMID
                    device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                    device.SetStreamSource(0, vb, 0);
                    device.Indices = ib;

                    mStack.MultiplyMatrixLocal(
                        Matrix.Scaling(new Vector3(0.5f, 0.5f, 0.5f)) *
                        Matrix.Translation(0, 2.0f + pyrUpDown / 2, 28)
                        );

                    device.Transform.World = mStack.Top;

                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 6, 0, 8);
                }
                mStack.Pop();

                mStack.Push();
                {
                    //BIG PYRAMID
                    device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                    device.SetStreamSource(0, vb, 0);
                    device.Indices = ib2;
                    mStack.MultiplyMatrixLocal(
                        Matrix.Scaling(new Vector3(1.5f, 1.5f, 1.5f)) *
                        Matrix.Translation(0.0f, 0.0f, 50.0f)
                        );
                    device.Transform.World = mStack.Top;
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 11, 0, 6);
                }
                mStack.Pop();




                mStack.Push();
                {
                    // TOR
                    Material torMaterial = new Material();
                    torMaterial.Ambient = Color.Wheat;
                    torMaterial.Diffuse = Color.Gold;
                    torMaterial.Specular = Color.White;
                    device.Material = torMaterial;

                    mStack.MultiplyMatrixLocal(
                        Matrix.RotationAxis(new Vector3(0, 1, 0), angleGlob) *
                        Matrix.Translation(0, 20.0f, 50.0f) *
                        Matrix.Scaling(1.5f, 1.0f, 1.0f)
                        );
                    device.Transform.World = mStack.Top;
                    device.Material = torMaterial;
                    tor.ComputeNormals();
                    tor.DrawSubset(0);
                }
                mStack.Pop();


                mStack.Push();
                {
                    // GLOB
                    Material globMaterial = new Material();
                    globMaterial.Ambient = Color.Yellow;
                    globMaterial.Diffuse = Color.Yellow;
                    globMaterial.Specular = Color.White;
                    //globMaterial.Emissive = Color.AliceBlue;
                    device.Material = globMaterial;

                    mStack.MultiplyMatrixLocal(
                        Matrix.Scaling(2.0f, 1.0f, 1.0f) *
                        Matrix.RotationAxis(new Vector3(1, 1, 0), angleGlob) *
                        Matrix.Translation(0.0f, 20.0f, 50.0f)
                        );
                    device.Transform.World = mStack.Top;
                    glob.DrawSubset(0);
                }
                mStack.Pop();


                mStack.Push();
                {
                    // PILLAR

                    Material pilMaterial = new Material();
                    pilMaterial.Ambient = Color.White;
                    pilMaterial.Diffuse = Color.White;
                    pilMaterial.Specular = Color.White;
                    device.Material = pilMaterial;

                    device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.RotationAxis(new Vector3(1, 0, 0), -(float)Math.PI / 2) *
                            Matrix.Scaling(1.5f, 4.0f, 1.7f) *
                            Matrix.Translation(2.0f, 2.0f, 34.0f)
                            );
                        device.Transform.World = mStack.Top;
                        pil.DrawSubset(0);
                    }
                    mStack.Pop();

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.RotationAxis(new Vector3(1, 0, 0), -(float)Math.PI / 2) *
                            Matrix.Scaling(1.5f, 4.0f, 1.7f) *
                            Matrix.Translation(-2.0f, 2.0f, 34.0f)
                            );
                        device.Transform.World = mStack.Top;
                        pil.DrawSubset(0);
                    }
                    mStack.Pop();

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.Scaling(1.5f, 5.0f, 1.5f) *
                            Matrix.RotationAxis(new Vector3(1, 0, 0), -(float)Math.PI / 2) *
                            Matrix.RotationAxis(new Vector3(0, 1, 0), -(float)Math.PI / 2) *
                            Matrix.Translation(0.0f, 4.4f, 34.0f)
                            );
                        device.Transform.World = mStack.Top;
                        pil.DrawSubset(0);
                    }
                    mStack.Pop();
                }
                mStack.Pop();

                mStack.Push();
                {
                    device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                    Vector3 norm = new Vector3(1.0f, 0.0f, 0.0f);
                    CustomVertex.PositionNormalTextured[] verts = new CustomVertex.PositionNormalTextured[4];
                    verts[0] = new CustomVertex.PositionNormalTextured(new Vector3(-1, 1, 0), norm, 0, 0);
                    verts[1] = new CustomVertex.PositionNormalTextured(new Vector3(-1, -1, 0), norm, 1, 0);
                    verts[2] = new CustomVertex.PositionNormalTextured(new Vector3(1, 1, 0), norm, 0, 1);
                    verts[3] = new CustomVertex.PositionNormalTextured(new Vector3(1, -1, 0), norm, 1, 1);

                    device.SetTexture(0, texI);

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.RotationAxis(new Vector3(0, 0, 1), -(float)Math.PI / 2) *
                            Matrix.Scaling(10.0f, 10.0f, 10.0f) *
                            Matrix.Translation(10.0f, 10.0f, -60.0f)
                            );
                        device.Transform.World = mStack.Top;
                        device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, verts);
                    }
                    mStack.Pop();

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.RotationAxis(new Vector3(0, 0, 1), -(float)Math.PI / 2) *
                            Matrix.RotationAxis(new Vector3(0, 1, 0), (float)Math.PI) *
                            Matrix.Scaling(10.0f, 10.0f, 10.0f) *
                            Matrix.Translation(-10.0f, 10.0f, -60.0f)
                            );
                        device.Transform.World = mStack.Top;
                        device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, verts);                    
                    }
                    mStack.Pop();

                    device.SetTexture(0, null);
                }
                mStack.Pop();
            }
            mStack.Pop();

            if (showInfo)
            {
                infoFont.DrawText(null, "Press Insert to Connect/Disconnect, Escape to Exit", new Rectangle(25, 25, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);
                infoFont.DrawText(null, "Press I to turn info On/Off", new Rectangle(25, 45, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);

                string ctrl = "Controls : W, A, S, D, C, Space, Arrows, Alt+Arrows, Home, Delete, 1, 2 ";

                if (wii != null)
                    if (wii.Connected)
                        ctrl = "Controls : Up, Down, Left, Right, Minus, Plus, Accel, A+Accel, Home, 1, 2 ";

                infoFont.DrawText(null, ctrl, new Rectangle(25, 65, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);

                int yPos = fullscreen ? ScreenHeight - 75 : Height - 75;

                infoFont.DrawText(null, myCamera.ToString(), new Rectangle(25, yPos, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);
            }

            device.EndScene();
            device.Present();
            this.Invalidate();
        }


        protected override void OnKeyUp(KeyEventArgs e)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
            {
                wii.ConnectAction();
                if(wii.Connected)
                    wii.setRumbling(500);
            }
            if (e.KeyCode == Keys.I)
            {
                showInfo = !showInfo;
            }
            if (e.KeyCode == Keys.Home)
            {
                myCamera.LookForward();
            }
            if (e.KeyCode == Keys.Delete)
            {
                myCamera.Reset();
            }
            if (e.KeyCode == Keys.Escape)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(CloseTheForm));
                }
                else
                {
                    CloseTheForm();
                }
            }
            if (e.KeyCode == Keys.D1)
            {
                sound2.Stop();
                sound1.SetCurrentPosition(0);
                sound1.Play(0, DSND.BufferPlayFlags.Default);
            }
            if (e.KeyCode == Keys.D2)
            {
                sound1.Stop();
                sound2.SetCurrentPosition(0);
                sound2.Play(0, DSND.BufferPlayFlags.Default);
            }
        }

        protected void readKeyboard()
        {
            DI.KeyboardState keys = keyb.GetCurrentKeyboardState();

            if (keys[DI.Key.A])
            {
                myCamera.MoveStrafeLeftRight(moveStep);
            }
            if (keys[DI.Key.D])
            {
                myCamera.MoveStrafeLeftRight(-moveStep);
            }
            if (keys[DI.Key.W])
            {
                myCamera.MoveForwardBackward(moveStep);
            }
            if (keys[DI.Key.S])
            {
                myCamera.MoveForwardBackward(-moveStep);
            }
            if (keys[DI.Key.C])
            {
                myCamera.MoveUpDown(-moveStep);
            }
            if (keys[DI.Key.Space])
            {
                myCamera.MoveUpDown(moveStep);
            }
            if (keys[DI.Key.Up])
            {
                myCamera.Pitch(0.02f);
            }
            if (keys[DI.Key.Down])
            {
                myCamera.Pitch(-0.02f);
            }
            if (keys[DI.Key.Left])
            {
                if (keys[DI.Key.LeftAlt])
                    myCamera.Yaw(0.02f);
                else 
                    myCamera.MoveViewRotateLeftRight(0.02f);
            }
            if (keys[DI.Key.Right])
            {
                if (keys[DI.Key.LeftAlt])
                    myCamera.Yaw(-0.02f);
                else
                myCamera.MoveViewRotateLeftRight(-0.02f);
            }

        }

        protected void readWii()
        {
            float accelX = wii.AccelarationX;
            float accelY = wii.AccelarationY;
            float accelZ = wii.AccelarationZ;

            accelX /= 20;
            accelY /= 20;            
            //accelZ = (accelZ-1)/20;

            accelX = Math.Abs(accelX) > 0.004 ? accelX : 0;
            accelY = Math.Abs(accelY) > 0.004 ? accelY : 0;
            //accelZ = Math.Abs(accelZ) > 0.004 ? accelZ : 0;

            Console.WriteLine(accelX+" "+accelY+" "+accelZ);

            if (wii.ButtonA)
                myCamera.Yaw(-accelX);
            else
                myCamera.MoveViewRotateLeftRight(-accelX);

            myCamera.Pitch((-accelY)*1.3f);
            //myCamera.Roll(-accelZ);
            //myCamera.RotateView(-accelZ,0.0f,0.0f,1.0f);

            //myCamera.RotateView(wii.AccelarationX / 20, 0.0f, 1.0f, 0.0f);
            //myCamera.RotateView(-wii.AccelarationY / 20, 1.0f, 0.0f, 0.0f);
            //myCamera.RotateView(wii.AccelarationZ, 0.0f, 0.0f, 1.0f);            

            if (wii.ButtonHome)
            {
                myCamera.LookForward();
            }

            if (wii.ButtonLeft)
            {
                myCamera.MoveStrafeLeftRight(moveStep);
            }
            if (wii.ButtonRight)
            {
                myCamera.MoveStrafeLeftRight(-moveStep);
            }
            if (wii.ButtonUp)
            {
                myCamera.MoveForwardBackward(moveStep);
            }
            if (wii.ButtonDown)
            {
                myCamera.MoveForwardBackward(-moveStep);
            }
            if (wii.ButtonMinus)
            {
                myCamera.MoveUpDown(-moveStep);
            }
            if (wii.ButtonPlus)
            {
                myCamera.MoveUpDown(moveStep);
            }
            if (wii.Button1)
            {
                sound2.Stop();
                sound1.Play(0, DSND.BufferPlayFlags.Default);
            }
            if( wii.Button2)
            {
                sound1.Stop();
                sound2.Play(0, DSND.BufferPlayFlags.Default);
            }            
        }

        protected void setAudio()
        {
            if (bMusic == null)
                return;

            float angle = myCamera.FindForwardMoveForwardViewAngle();
            int balance = 0;
            int side = 0;

            bMusic.Balance = 0;

            if (angle > 0)//looking left
                side = 1;
            else// looking right
                side = -1;

            angle = Math.Abs(angle);
            angle = 10000*(float)Math.Sin(angle);

            balance = side * (int)angle;
            bMusic.Balance = balance;

            /*
            Vector3 pos = myCamera.Position;
            float distance = GeometryUtil.DistanceBetween(pos, new Vector3(0,0,0));
            
            // -10000  is minimum vol, 0 maximum
            // 50 is maximum distance for audio
            int vol = (int)(distance*500/50);
            vol = vol > 10000 ? 10000 : vol;
            bMusic.Volume = -vol;
            System.Console.WriteLine(" volume : " + bMusic.Volume);
            System.Console.WriteLine(" distance : " + distance);
            */
        }

        protected void setAnimationParameters()
        {
            if (pyrUpDown > 1 || pyrUpDown < -1)
                pyrUpDownStep *= -1;

            pyrUpDown += pyrUpDownStep;

            if (angleTor > 0.3 || angleTor < -0.3)
                angleTorStep *= -1;

            angleTor += angleTorStep;

            if (angleGlob >= 2 * Math.PI)
                angleGlob = 0.0f;

            angleGlob += angleGlobStep;

        }

        public void CloseTheForm()
        {
            if (wii.Connected)
                wii.ConnectAction();
            Cursor.Show();
            Close();
        }

        protected void TimerEventProcessor(object source, ElapsedEventArgs e)
        {
            readKeyboard();

            if(wii.Connected)
                readWii();

            setAnimationParameters();
            setAudio();
        }

        static void Main()
        {
            {
                Form1 f1 = new Form1();
                f1.InitializeGraphicsAndCamera();
                f1.InitializeAudio();
                f1.InitializeKeyboard();
                f1.InitializeWii();
                f1.InitializeTimer();
                f1.Show();
                Application.Run(f1);
            }
        }
    }
}
