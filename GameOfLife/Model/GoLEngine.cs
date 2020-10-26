using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife.Model
{
    class GoLEngine
    {
        private Random rnd = new Random();

        public event EventHandler OnEngineUpdate;

        protected virtual void RaiseEngineUpdate(EventArgs e)
        {
            EventHandler handler = OnEngineUpdate;
            handler?.Invoke(this, e);
        }

        public int Delay { get; set; } = 0;

        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }

        public int ScreenWidth { get; set; } = 200;
        public int ScreenHeigth { get; set; } = 100;
        public int PixelSize { get; set; } = 1;

        public Pixel[] m_output { get; set; }
        private int[] m_state { get; set; }

        public GoLEngine() 
        {
            Reset();
        }

        public async void Start()
        {
            Reset();

            IsPaused = false;
            IsRunning = true;

            System.Threading.Thread.Sleep(100);

            await Render();
        }
        
        public void Stop()
        {
            IsRunning = false;
            IsPaused = false;

            System.Threading.Thread.Sleep(100);
            
            Init();
            
            Debug.Print("stopped");
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                IsPaused = true;
            }
            else
            {
                IsPaused = false;
            }
        }

        private void Init()
        {
            m_output = new Pixel[ScreenWidth * ScreenHeigth];
            m_state = new int[ScreenWidth * ScreenHeigth];

            for (int i = 0; i < ScreenWidth * ScreenHeigth; i++)
            {
                m_state[i] = 0;
                m_output[i] = new Pixel(m_state[i]);
            }

            RaiseEngineUpdate(new EventArgs());
        }

        private void Reset()
        {
            m_output = new Pixel[ScreenWidth * ScreenHeigth];
            m_state = new int[ScreenWidth * ScreenHeigth];
            Delay = 0;
            for (int i = 0; i < ScreenWidth * ScreenHeigth; i++)
            {
                m_state[i] = rnd.Next() % 2;
                m_output[i] = new Pixel(m_state[i]);
            }

            System.Threading.Thread.Sleep(100);

            RaiseEngineUpdate(new EventArgs());
        }

        private Task Render()
        {
            return Task.Factory.StartNew(() =>
            {
                while (IsRunning)
                {
                    while (IsPaused)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    //Code here..
                    for (int i = 0; i < ScreenWidth * ScreenHeigth; i++)
                    {
                        m_output[i].State = m_state[i];
                    }

                    for (int x = 1; x < ScreenWidth-1; x++)
                    {
                        for (int y = 1; y < ScreenHeigth -1; y++)
                        {

                            int nNeighbours = cell(x - 1, y - 1) + cell(x - 0, y - 1) + cell(x + 1, y - 1) +
                                              cell(x - 1, y + 0) +          0         + cell(x + 1, y + 0) +
                                              cell(x - 1, y + 1) + cell(x + 0, y + 1) + cell(x + 1, y + 1)
                                              ;

                            if (cell(x, y) == 1)
                            {
                                //Wenn cell 2 oder 3 Nachbarn hat..
                                if (nNeighbours == 2 || nNeighbours == 3)
                                {
                                    m_state[y * ScreenWidth + x] = 1;
                                }
                                else
                                {
                                    m_state[y * ScreenWidth + x] = 0;
                                }
                            }
                            else //Verstorben
                            {
                                //Voraussetzungen für Geburt erfüllt..
                                if (nNeighbours == 3)
                                {
                                    m_state[y * ScreenWidth + x] = 1;
                                }
                            }

                            m_output[y * ScreenWidth + x].Location.X = x;
                            m_output[y * ScreenWidth + x].Location.Y = y;
                        }
                    }


                    Thread.Sleep(Delay);
                    //Debug.Print("running");

                    RaiseEngineUpdate(new EventArgs());
                }
            });
        }

        private int cell(int x, int y)
        {
            return m_output[y * ScreenWidth + x].State;
        }
    }
}
