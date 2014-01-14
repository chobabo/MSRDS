using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace MazeExplorer
{
    //状態変数の上限、下限, 상태변수의 상한, 하한을 설정
    public struct LIMIT
    {
        public int x;
        public int y;
        public int vx;
        public int vy;

        public LIMIT(int _x, int _y, int _vx, int _vy)
        {
            x = _x;
            y = _y;
            vx = _vx;
            vy = _vy;
        }
    }

    // ノイズの最大値 , 노이즈를 설정
    public struct NOISE
    {
        public int x;
        public int y;
        public int vx;
        public int vy;

        public NOISE(int _x, int _y, int _vx, int _vy)
        {
            x = _x;
            y = _y;
            vx = _vx;
            vy = _vy;
        }
    }


    class pfilter
    {
        public pfilter() { }

        const int RAND_MAX = 32767;
        Random rand = new Random((int)DateTime.Now.Ticks);

        // 各変数の上限、下限
        LIMIT upper, lower;

        // ノイズ
        NOISE noise;

        //パーティクル数
        int num;

        //パーティクル配列
        public List<Particle> particles = new List<Particle>();
        // パーティクル配列（事前推定用）
        public List<Particle> pre_particles = new List<Particle>();

        public pfilter(int _num, LIMIT _upper, LIMIT _lower, NOISE _noise)
        {
            
            this.num = _num;

            this.upper = _upper;
            this.lower = _lower;
            this.noise = _noise;

            for (int i = 0; i < num; i++)
            {
                int x = rand.Next(RAND_MAX) % (upper.x - lower.x) + lower.x;
                int y = rand.Next(RAND_MAX) % (upper.y - lower.y) + lower.y;
                int vx = rand.Next(RAND_MAX) % (upper.vx - lower.vx) + lower.vx;
                int vy = rand.Next(RAND_MAX) % (upper.vy - lower.vy) + lower.vy;
                double w = 1.0 / (double)num;

                Particle p = new Particle(x, y, vx, vy, w);
                particles.Add(p);
                //particles.Add(p);
            }

            for (int i = 0; i < num; i++)
            {
                pre_particles.Add(particles[i]);
                //pre_particles.Add(particles[i]);
            }
        }

        //状態遷移モデルを適用
        //事前推定（遷移モデルの適用）から次のパーティクルの位置を予測
        public void predict()
        {
            int pre_x, pre_y;
            for (int i = 0; i < num; i++)
            {
                //ランダムノイズの更新
                NOISE n;
                n.x = (int)(((double)rand.Next(RAND_MAX) / ((double)RAND_MAX + 1)) * (double)noise.x * 2 - (double)noise.x);
                n.y = (int)(((double)rand.Next(RAND_MAX) / ((double)RAND_MAX + 1)) * (double)noise.y * 2 - (double)noise.y);
                n.vx = (int)(((double)rand.Next(RAND_MAX) / ((double)RAND_MAX + 1)) * (double)noise.vx * 2 - (double)noise.vx);
                n.vy = (int)(((double)rand.Next(RAND_MAX) / ((double)RAND_MAX + 1)) * (double)noise.vy * 2 - (double)noise.vy);

                // 等速直線運動をモデルに移動したと予測する
                pre_x = particles[i].get_x + particles[i].get_vx + n.x;
                pre_particles[i].set_x(pre_x);
                pre_y = particles[i].get_y + particles[i].get_vy + n.y;
                pre_particles[i].set_y(pre_y);
                pre_particles[i].set_vx(n.vx);
                pre_particles[i].set_vy(n.vy);

                //下限より小さいとき、下限の値とする
                if (pre_particles[i].get_x < lower.x) pre_particles[i].set_x(lower.x);
                if (pre_particles[i].get_y < lower.y) pre_particles[i].set_y(lower.y);
                if (pre_particles[i].get_vx < lower.vx) pre_particles[i].set_vx(lower.vx);
                if (pre_particles[i].get_vy < lower.vy) pre_particles[i].set_vy(lower.vy);

                //上限より大きいとき、上限の値とする
                if (pre_particles[i].get_x > upper.x) pre_particles[i].set_x(upper.x);
                if (pre_particles[i].get_y > upper.y) pre_particles[i].set_y(upper.y);
                if (pre_particles[i].get_vx > upper.vx) pre_particles[i].set_vx(upper.vx);
                if (pre_particles[i].get_vy > upper.vy) pre_particles[i].set_vy(upper.vy);
            }

        }


        //予測後の各パーティクルについて重み付けを行う
        public void weight(int centerX, int centerY)
        {
            for (int i = 0; i < num; i++)
            {
                int x = pre_particles[i].get_x;
                int y = pre_particles[i].get_y;
                pre_particles[i].setWeight(calcLikelihood(x, y, centerX, centerY));
            }

            //正規化
            double sum = 0.0;
            for (int i = 0; i < num; i++)
            {
                sum += pre_particles[i].getWeight;
            }
            for (int i = 0; i < num; i++)
            {
                double w = pre_particles[i].getWeight / sum;
                pre_particles[i].setWeight(w);
            }
        }

        // 尤度の計算
        public double calcLikelihood(int x, int y, int centerX, int centerY)
        {
            double result = 0.0;
            
            //내꺼에 맞게 다시 코드를 작성해야 한다
            double dist = 0.0;
            //시그마 값이 작을수록 결과 값의 범위가 작아진다.
            double sigma = 10.0;
            dist = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
            
            result = 1.0 / (Math.Sqrt(2.0 * Math.PI) * sigma) * Math.Exp(-dist * dist / (2.0 * sigma * sigma));


            return result;
        }

        //重みに基づき再サンプリングする（ルーレット選択）
        public void resample()
        {
            // 累積重み（＝ルーレット）
            double[] w = new double[num];
            w[0] = pre_particles[0].getWeight;
            for (int i = 1; i < num; i++)
            {
                w[i] = w[i - 1] + pre_particles[i].getWeight;
            }

            for (int i = 0; i < num; i++)
            {
                //이부분 한번 다시 봐야 한다. 
                double darts = ((double)rand.Next(RAND_MAX) / ((double)RAND_MAX + 1));
                for (int j = 0; j < num; j++)
                {
                    if (darts > w[j]) 
                    { 
                        continue; 
                    }
                    else
                    {
                        // リサンプリング
                        particles[i].set_x(pre_particles[j].get_x);
                        particles[i].set_y(pre_particles[j].get_y);
                        particles[i].set_vx(pre_particles[j].get_vx);
                        particles[i].set_vy(pre_particles[j].get_vy);

                        particles[i].setWeight(0.0);
                        break;
                    }

                }
            }
        }

        //パーティクルの重みつき平均を推定結果
        public void measure(Particle result)
        {
            double x = 0, y = 0, vx = 0, vy = 0;
            for (int i = 0; i < num; i++)
            {
                x += (double)particles[i].get_x * particles[i].getWeight;
                y += (double)particles[i].get_y * particles[i].getWeight;
                vx += (double)particles[i].get_vx * particles[i].getWeight;
                vy += (double)particles[i].get_vy * particles[i].getWeight;
            }

            result.set_x((int)x);
            result.set_y((int)y);
            result.set_vx((int)vx);
            result.set_vy((int)vy);
        }


    }
}
