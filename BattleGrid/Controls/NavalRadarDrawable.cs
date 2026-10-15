using Microsoft.Maui.Graphics;

namespace BattleGrid.Controls;

// Decorative tactical-map art for MTP-10; actual play logic comes later.
public sealed class NavalRadarDrawable : IDrawable
{
    public void Draw(ICanvas c, RectF bounds)
    {
        float s = Math.Min(bounds.Width - 18, bounds.Height - 10);
        if (s <= 0) return;
        float q = s / 10f;
        float x = bounds.X + (bounds.Width - s) / 2;
        float y = bounds.Y + (bounds.Height - s) / 2;
        c.FillColor = Color.FromArgb("#0A527A");
        c.FillRoundedRectangle(x, y, s, s, 9);
        c.StrokeColor = Color.FromArgb("#287DA2");
        c.StrokeSize = 1;
        for (int row=0; row<10; row++)
            for (int col=0; col<10; col++)
            {
                if ((row+col)%3 == 0)
                {
                    c.FillColor = Color.FromArgb("#105F89");
                    c.FillRectangle(x+col*q+1, y+row*q+1, q-2, q-2);
                }
            }
        c.StrokeColor = Color.FromArgb("#318FB5");
        for (int i=0;i<=10;i++)
        {
            c.DrawLine(x+i*q,y,x+i*q,y+s);
            c.DrawLine(x,y+i*q,x+s,y+i*q);
        }
        c.StrokeColor = Color.FromArgb("#55ACCB").WithAlpha(.46f);
        c.StrokeSize=1.2f;
        for (int i=1;i<=3;i++) c.DrawCircle(x+s/2,y+s/2,i*s/8);
        // Ships, drawn as top-view hulls rather than passenger-ship emoji.
        Ship(c,x+q*.35f,y+q*1.3f,q*.67f,q*4.0f,true);
        Ship(c,x+q*5.3f,y+q*2.4f,q*3.55f,q*.73f,false);
        Ship(c,x+q*3.8f,y+q*7.15f,q*2.9f,q*.7f,false);
        Target(c,x+q*7.5f,y+q*6.6f,q*.33f);
        Target(c,x+q*2.6f,y+q*5.5f,q*.33f);
        Cross(c,x+q*8.5f,y+q*1.25f,q*.22f);
        c.StrokeColor = Color.FromArgb("#B8ECFA").WithAlpha(.62f);
        c.StrokeSize = 2;
        c.DrawLine(x+s/2,y+s/2,x+s*.75f,y+s*.27f);
    }
    private static void Ship(ICanvas c,float x,float y,float w,float h,bool vertical)
    {
        float radius=Math.Min(w,h)*.4f;
        c.FillColor=Color.FromArgb("#061F36");
        c.FillRoundedRectangle(x+2,y+3,w,h,radius);
        c.FillColor=Color.FromArgb("#D9F1F9");
        c.FillRoundedRectangle(x,y,w,h,radius);
        c.FillColor=Color.FromArgb("#6E9AB0");
        float inset=Math.Min(w,h)*.2f;
        c.FillRoundedRectangle(x+inset,y+inset,w-2*inset,h-2*inset,Math.Max(2,radius*.4f));
        c.FillColor=Color.FromArgb("#E8F9FF");
        if(vertical)
        {
            c.FillRoundedRectangle(x+w*.28f,y+h*.42f,w*.44f,h*.18f,3);
            c.FillRectangle(x+w*.4f,y+h*.16f,w*.2f,h*.15f);
        }
        else
        {
            c.FillRoundedRectangle(x+w*.42f,y+h*.28f,w*.18f,h*.44f,3);
            c.FillRectangle(x+w*.7f,y+h*.4f,w*.12f,h*.2f);
        }
    }
    private static void Target(ICanvas c,float x,float y,float r)
    {
        c.StrokeColor=Color.FromArgb("#FFB548");c.StrokeSize=2.5f;
        c.DrawCircle(x,y,r);c.DrawCircle(x,y,r*.4f);
        c.DrawLine(x-r*1.5f,y,x-r*.7f,y);c.DrawLine(x+r*.7f,y,x+r*1.5f,y);
        c.DrawLine(x,y-r*1.5f,x,y-r*.7f);c.DrawLine(x,y+r*.7f,x,y+r*1.5f);
    }
    private static void Cross(ICanvas c,float x,float y,float r)
    {
        c.StrokeColor=Color.FromArgb("#FF9F2E");c.StrokeSize=3.3f;
        c.DrawLine(x-r,y-r,x+r,y+r);c.DrawLine(x-r,y+r,x+r,y-r);
    }
}
