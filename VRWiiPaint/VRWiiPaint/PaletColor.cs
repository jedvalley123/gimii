﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace VRWiiPaint
{
    //======================================================================================================
    //          Klasa MyPalet:
    //  Ima 5 atributa:
    //                  int x - x pozicija
    //                  int y - y pozicija
    //                  int w - sirinu
    //                  int h - visinu 
    //                  Color myColor - boju
    //  i konstruktor sa parametrima za sve atribute.
    //======================================================================================================
    class MyPalet
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public Color myColor;

        public MyPalet()
        { 
        }

        public MyPalet(int _x, int _y, int _w, int _h, Color _color)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            myColor = _color;
        }
    }

    //======================================================================================================
    //          Klasa PaletColor nasledjuje klasu MyPalet:
    //  Nema dodatni atributa, ima konstruktor sa parametrima za sve atribute i funkciju InsidePalet.
    //======================================================================================================
    class PaletColor: MyPalet
    {
        public PaletColor(int _x, int _y, int _w, int _h, Color _color)
        {   
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            myColor = _color;

        }

        //======================================================================================================
        //          Funkcija InsidePalet:
        //  Ima 2 parametra:
        //                 int _x - pozicija x
        //                 int _y - pozicija y
        //  Funkcia ispituje da li se tacka na poziciji _x,_y nalazi unutar PaletColor i ako se nalazi vraca ture,
        //  inace false.
        //======================================================================================================
        public bool UnutarPalet(int _x, int _y)
        {

            if( ( _x >= x) && (_x <= (x+w))
                && (_y >= y) && (_y<=(y + h)) )
                return true;

            return false;
        }
    }


    //======================================================================================================
    //          Klasa Dugme:
    //  Ima 7 atributa:
    //                  int x - x pozicija
    //                  int y - y pozicija
    //                  int w - sirinu
    //                  int h - visinu 
    //                  String ime - ime dugmeta 
    //                  Color boja - boja dugmeta
    //                  Color selectBoja - boja dugmeta kada je selektovano
    //  i konstruktor sa parametrima za sve atribute i funkcije UnutarDugmeta, NacrtajDugme
    //======================================================================================================
    class Dugme
    {
        int x;
        int y;
        int w;
        int h;
        String ime;
        Color boja;
        Color selectBoja;
        
        public Dugme(int _x, int _y, int _w, int _h, String _ime, Color _boja, Color _selectBoja)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            ime = _ime;
            boja = _boja;
            selectBoja = _selectBoja;
        }


        //======================================================================================================
        //          Funkcija UnutarDugmeta:
        //  Ima 2 parametra:
        //                 int _x - pozicija x
        //                 int _y - pozicija y
        //  Funkcia ispituje da li se tacka na poziciji _x,_y nalazi unutar Dugme i ako se nalazi vraca ture,
        //  inace false.
        //======================================================================================================
        public bool UnutarDugmeta(int _x, int _y)
        {
            if ((_x >= x) && (_x <= (x + w))
                && (_y >= y) && (_y <= (y + h)))
                return true;

            return false;
        }


        //======================================================================================================
        //          Funkcija NacrtajDugme:
        //  Ima 2 parametra:
        //                 bool neselektovano - true ako je dugme neselektovano ako je selektovano false
        //                 Graphics _graphics - Graphics kojom crtamo dugme
        //  Funkcia crta dugme i nema povratnu vrednosts.
        //======================================================================================================
        public void NacrtajDugme(bool neselektovano, Graphics _graphics)
        {

            if (neselektovano)
                _graphics.FillRectangle(new SolidBrush(boja), new Rectangle(x, y, w, h));
            else
                _graphics.FillRectangle(new SolidBrush(selectBoja), new Rectangle(x, y, w, h));
            _graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x - 2, y - 2, w + 2, h + 2));
            _graphics.DrawString(ime, new Font("Arial", 10), new SolidBrush(Color.Black), x + 2, y + 2);

        }


    }


    //======================================================================================================
    //          Klasa DugmeVeceManje:
    //  Ima 7 atributa:
    //                  int x - x pozicija
    //                  int y - y pozicija
    //                  int w - sirinu
    //                  int h - visinu 
    //                  int vrednost - vrednost koju ima dugme vece manje
    //                  Color boja - boja dugmeta
    //                  Color selectBoja - boja dugmeta kada je selektovano
    //  i konstruktor sa parametrima za sve atribute i funkcije incVrednost, decVrednost, UnutarDugmeta,
    //  NacrtajDugme, NacrtajSelektovanoManjeDugme, NacrtajSelektovanoVeceDugme
    //======================================================================================================
    class DugmeVeceManje
    {
        int x;
        int y;
        int w;
        int h;
        int vrednost;
        Color boja;
        Color selectBoja;
        

        public DugmeVeceManje(int _x, int _y, int _w, int _h, int _vrednost, Color _boja, Color _selectBoja)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            vrednost = _vrednost;
            boja = _boja;
            selectBoja = _selectBoja;
        }

        //======================================================================================================
        //          Funkcija incVrednost:
        //  Ako atribut vrednost ima vrednost manju od 40 povecavamo ga za 4.
        //======================================================================================================
        public void incVrednost()
        {
            if (vrednost < 40)
                vrednost += 4;
        }


        //======================================================================================================
        //          Funkcija decVrednost:
        //  Ako atribut vrednost ima vrednost vecu od 4 smanjimo ga za 4.
        //======================================================================================================
        public void decVrednost()
        {
            if (vrednost > 4)
                vrednost -= 4;
        }

        //======================================================================================================
        //          Funkcija UnutarDugmeta:
        //  Ima 2 parametra:
        //                 int _x - pozicija x
        //                 int _y - pozicija y
        //                 Graphics g - Graphics kojom crtamo dugme
        //  Funkcia ispituje da li se tacka na poziciji _x,_y nalazi unutar Dugme i ako se nalazi i ako je
        //  pogodjeno manje poziva funkciju decVrednost, ako je pogodjeno vece poziva funkciju incVrednost
        //  pa onde crta dugme i vraca int vrednost.
        //======================================================================================================
        public int UnutarDugmeta(int _x, int _y, Graphics g)
        {

            if ((_x >= x) && (_x <= (x + 40))
                && (_y >= y) && (_y <= (y + h)))
            {//dugme manje
                this.decVrednost();
                this.NacrtajSelektovanoManjeDugme(g);
            }
            else
            {
                if ((_x >= x) && (_x <= (x + w))
                && (_y >= y) && (_y <= (y + h)))
                {//dugme vece
                    this.incVrednost();
                    this.NacrtajSelektovanoVeceDugme(g);
                }
                else
                {//promaseno
                    this.NacrtajDugme(g);
                }
            }

            return vrednost;
        }


        //======================================================================================================
        //          Funkcija NacrtajDugme:
        //  Ima 1 parametra:
        //                 Graphics _graphics - Graphics kojom crtamo dugme
        //  Funkcia crta dugme neselektovano i nema povratnu vrednosts.
        //======================================================================================================
        public void NacrtajDugme(Graphics _graphics)
        {

            _graphics.FillRectangle(new SolidBrush(boja), new Rectangle(x, y, w, h));
            _graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x - 2, y - 2, w + 2, h + 2));

            _graphics.DrawLine(new Pen(Color.Black, 1), x + 41, y - 2, x + 41, y + h);
            _graphics.DrawLine(new Pen(Color.Black, 1), x + w - 41, y - 2, x + w - 41, y + h);

            _graphics.FillRectangle(new SolidBrush(selectBoja), new Rectangle(x + 42, y, w - 42 * 2, h));

            _graphics.DrawString("<", new Font("Arial", 30), new SolidBrush(Color.Black), x, y);
            String v = "" + vrednost;
            _graphics.DrawString(v, new Font("Arial", 30), new SolidBrush(Color.Black), x + 37, y);
            _graphics.DrawString(">", new Font("Arial", 30), new SolidBrush(Color.Black), x + w - 40, y + 2);

        }


        //======================================================================================================
        //          Funkcija NacrtajDugme:
        //  Ima 1 parametra:
        //                 Graphics _graphics - Graphics kojom crtamo dugme
        //  Funkcia crta dugme ali selektovano manje dugme i nema povratnu vrednosts.
        //======================================================================================================
        private void NacrtajSelektovanoManjeDugme(Graphics _graphics)
        {

            _graphics.FillRectangle(new SolidBrush(selectBoja), new Rectangle(x, y, w, h));
            _graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x - 2, y - 2, w + 2, h + 2));

            _graphics.DrawLine(new Pen(Color.Black, 1), x + 41, y - 2, x + 41, y + h);
            _graphics.DrawLine(new Pen(Color.Black, 1), x + w - 41, y - 2, x + w - 41, y + h);

            _graphics.FillRectangle(new SolidBrush(boja), new Rectangle(x + w - 40, y, 40, h));

            _graphics.DrawString("<", new Font("Arial", 30), new SolidBrush(Color.Black), x, y);
            String v = "" + vrednost;
            _graphics.DrawString(v, new Font("Arial", 30), new SolidBrush(Color.Black), x + 37, y);
            _graphics.DrawString(">", new Font("Arial", 30), new SolidBrush(Color.Black), x + w - 40, y + 2);

        }

        //======================================================================================================
        //          Funkcija NacrtajSelektovanoVeceDugme:
        //  Ima 1 parametra:
        //                 Graphics _graphics - Graphics kojom crtamo dugme
        //  Funkcia crta dugme ali selektovano veve dugme i nema povratnu vrednosts.
        //======================================================================================================
        private void NacrtajSelektovanoVeceDugme(Graphics _graphics)
        {

            _graphics.FillRectangle(new SolidBrush(selectBoja), new Rectangle(x, y, w, h));
            _graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x - 2, y - 2, w + 2, h + 2));

            _graphics.DrawLine(new Pen(Color.Black, 1), x + 41, y - 2, x + 41, y + h);
            _graphics.DrawLine(new Pen(Color.Black, 1), x + w - 41, y - 2, x + w - 41, y + h);

            _graphics.FillRectangle(new SolidBrush(boja), new Rectangle(x, y, 40, h));

            _graphics.DrawString("<", new Font("Arial", 30), new SolidBrush(Color.Black), x, y);
            String v = "" + vrednost;
            _graphics.DrawString(v, new Font("Arial", 30), new SolidBrush(Color.Black), x + 37, y);
            _graphics.DrawString(">", new Font("Arial", 30), new SolidBrush(Color.Black), x + w - 40, y + 2);

        }


    }



    //======================================================================================================
    //          Klasa MyPalet:
    //  Ima 5 atributa:
    //                  List<PaletColor> boje - Listu boja PaletColor
    //                  Dugme btnIzlaz - dugme za izlaz 
    //                  Dugme btnSnimanje - dugme za snimanje
    //                  Dugme btnNovi - dugme za novi fajl
    //                  DugmeVeceManje btnManjeVece - dugme za vecu manju vrednost
    //  i konstruktor bez parametara i funkcije NacrtajPalets, ChangeColor, NacrtajTrenutnuBoju, DrawPositionInfo, 
    //  DrawPositionInfoDefault ,DrawClickInfo.
    //======================================================================================================
    class Palets
    {
        public List<PaletColor> boje;
        public Dugme btnIzlaz;
        public Dugme btnSnimanje;
        public Dugme btnNovi;
        public DugmeVeceManje btnManjeVece;

        public Palets()
        {
            boje = new List<PaletColor>();

        }


        //======================================================================================================
        //          Funkcija NacrtajPalets:
        //  Ima 4 parametra:
        //                 Graphics g - Graphics kojom crtamo paletu
        //                 Color _trenutnaBoja - trenutna boja kojom bojimo
        //                 int _trenutnaBojaX - x pozicija trenutna boje kojom bojimo
        //                 int _trenutnaBojaY - y pozicija trenutna boje kojom bojimo
        //  Funkcia crta svaku boju iz niza boja colors i crta trenutnu boju pozivom funkcije NacrtajTrenutnuBoju
        //======================================================================================================
        public void NacrtajPalets(Graphics g, Color _trenutnaBoja, int _trenutnaBojaX, int _trenutnaBojaY)
        {
            foreach (PaletColor pc in boje)
            {
                // iscrtaj boju
                g.FillRectangle(new SolidBrush(pc.myColor), new Rectangle(pc.x, pc.y, pc.w, pc.h));

                // iscrtaj crni ram
                g.DrawRectangle(new Pen(Color.Black, 1), pc.x - 2, pc.y - 2, pc.w + 2, pc.h + 2);
            }

            // iscrtaj trenutnu boju
            NacrtajTrenutnuBoju(_trenutnaBoja, g, _trenutnaBojaX, _trenutnaBojaY);


        }


        //======================================================================================================
        //          Funkcija PromeniBoju:
        //  Ima 4 parametra:
        //                 int _x - pozicija x
        //                 int _y - pozicija y
        //                 Color staraBoja - stara trenutna boja kojom bojimo
        //                 Graphics g - Graphics kojom crtamo paletu
        //                 int _trenutnaBojaX - x pozicija trenutna boje kojom bojimo
        //                 int _trenutnaBojaY - y pozicija trenutna boje kojom bojimo
        //  Funkcia ispituje da li se na poziciji _x, _y nalazi neka boja (PaletColor) ako se nalazi crta
        //  nju kao trenutnu boju kojom crtamo i vraca je kao povratnu vrednost. Ako ne vraca parametar staraBoja
        //  kao povratnu vrednost.
        //======================================================================================================
        public Color PromeniBoju(int _x, int _y, Color staraBoja, Graphics g, int _trenutnaBojaX, int _trenutnaBojaY)
        {
            foreach (PaletColor pc in boje)
            {
                if (pc.UnutarPalet(_x, _y))
                {
                    // iscrtaj trenutnu boju
                    NacrtajTrenutnuBoju(pc.myColor, g, _trenutnaBojaX, _trenutnaBojaY);
                    return pc.myColor;
                }
            }

            return staraBoja;
       }


        //======================================================================================================
        //          Funkcija NacrtajTrenutnuBoju:
        //  Ima 4 parametra:
        //                 Graphics g - Graphics kojom crtamo trenutnu boju koju bojimo
        //                 Color _trenutnaBoja - trenutna boja kojom bojimo
        //                 int _trenutnaBojaX - x pozicija trenutna boje kojom bojimo
        //                 int _trenutnaBojaY - y pozicija trenutna boje kojom bojimo
        //======================================================================================================
        public void NacrtajTrenutnuBoju(Color _trenutnaBoja, Graphics g, int _trenutnaBojaX, int _trenutnaBojaY)
       {

           g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(_trenutnaBojaX - 2, _trenutnaBojaY - 2, 102, 102));
           g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(_trenutnaBojaX, _trenutnaBojaY, 98, 98));
           g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(_trenutnaBojaX + 2, _trenutnaBojaY + 2, 94, 94));
           g.FillRectangle(new SolidBrush(_trenutnaBoja), new Rectangle(_trenutnaBojaX + 3, _trenutnaBojaY + 3, 93, 93));

       }


        //======================================================================================================
        //          Funkcija NacrtajPozicijaInfo:
        //  Ima 4 parametra:
        //                 int xPos - pozicija x na kojo se kursor nalazi
        //                 int yPos - pozicija y na kojo se kursor nalazi
        //                 int x - pozicija x na kojo crtamo
        //                 int y - pozicija y na kojo crtamo
        //                 int w - sirina pozicija infa
        //                 int h - visina pozicija infa
        //                 Color boja - boja koju ce da ima
        //                 Graphics g - Graphics kojom crtamo trenutnu boju koju bojimo
        //  Funkcija crta info za poziciju x, y, kao i njenu vrednost. 
        //======================================================================================================
        public void NacrtajPozicijaInfo(int xPos, int yPos, int x, int y, int w, int h, Color boja, Graphics _graphics)
        {
            _graphics.FillRectangle(new SolidBrush(boja), new Rectangle(x, y, w, h));
            _graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x-2, y-2, w+2, h+2));

            string text = " X:" + xPos.ToString() + ".0   , Y:" + yPos.ToString() + ".0";

            _graphics.DrawString(text, new Font("Arial",10), new SolidBrush(Color.Black), x+2,y+2);

           
        }


        //======================================================================================================
        //          Funkcija NacrtajPozicijaInfoDefault:
        //  Ima 4 parametra:
        //                 int x - pozicija x na kojo crtamo
        //                 int y - pozicija y na kojo crtamo
        //                 int w - sirina pozicija infa
        //                 int h - visina pozicija infa
        //                 Color boja - boja koju ce da ima
        //                 Graphics g - Graphics kojom crtamo trenutnu boju koju bojimo
        //  Funkcija crta info za poziciju x, y bez vrednost te pozicije. 
        //======================================================================================================
        public void NacrtajPozicijaInfoDefault(int x, int y, int w, int h, Color boja, Graphics g)
        {
            g.FillRectangle(new SolidBrush(boja), new Rectangle(x, y, w, h));
            g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x - 2, y - 2, w + 2, h + 2));

            string text = " X:     , Y:";

            g.DrawString(text, new Font("Arial", 10), new SolidBrush(Color.Black), x + 2, y + 2);
        }


        //======================================================================================================
        //          Funkcija NacrtajKlikInfo:
        //  Ima 4 parametra:
        //                 int x - pozicija x na kojo crtamo
        //                 int y - pozicija y na kojo crtamo
        //                 int w - sirina klik infa
        //                 int h - visina klik infa
        //                 Color boja - boja kada nije kliknuto
        //                 Color klikBoja - boja kada je kliknuto
        //                 Graphics g - Graphics kojom crtamo trenutnu boju koju bojimo
        //  Funkcija crta elipsu klikBoja boje ako ima klika ili boja boje ako nema klika na poziciji x, y 
        //======================================================================================================
        public void NacrtajKlikInfo(int x, int y, int w, int h, Color boja, Color klikBoja, Graphics g)
        {
            g.FillRectangle(new SolidBrush(boja), new Rectangle(x, y, w, h));
            g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(x - 2, y - 2, w + 2, h + 2));

            string text = " Klik: ";

            g.DrawString(text, new Font("Arial", 10), new SolidBrush(Color.Black), x + 2, y + 2);
            g.FillEllipse(new SolidBrush(klikBoja), new Rectangle(x + w / 2, y + 1, 17, 17));
            g.DrawEllipse(new Pen(Color.Black, 1), new Rectangle(x + w / 2, y + 1, 17, 17));
            
        }
    }

  
}
