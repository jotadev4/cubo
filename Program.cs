using System;
using System.Threading;

class Program
{
    static float AnguloA, AnguloB, AnguloC;
    static float larguraCubo = 20;
    static int larguraTela = 160, alturaTela = 44;
    static float[] bufferProfundidade = new float[160 * 44];
    static char[] bufferTela = new char[160 * 44];
    static char caractereFundo = '.';
    static int distanciaDaCamera = 100;
    static float deslocamentoHorizontal;
    static float fatorEscala = 40;
    static float velocidadeIncremento = 0.6f;

    static float coordX, coordY, coordZ;
    static float inversoZ;
    static int posX, posY;
    static int indice;
    static bool primeiroCiclo = true;

    static float CalcularX(int i, int j, int k)
    {
        return j * MathF.Sin(AnguloA) * MathF.Sin(AnguloB) * MathF.Cos(AnguloC) - k * MathF.Cos(AnguloA) * MathF.Sin(AnguloB) * MathF.Cos(AnguloC) +
               j * MathF.Cos(AnguloA) * MathF.Sin(AnguloC) + k * MathF.Sin(AnguloA) * MathF.Sin(AnguloC) + i * MathF.Cos(AnguloB) * MathF.Cos(AnguloC);
    }

    static float CalcularY(int i, int j, int k)
    {
        return j * MathF.Cos(AnguloA) * MathF.Cos(AnguloC) + k * MathF.Sin(AnguloA) * MathF.Cos(AnguloC) -
               j * MathF.Sin(AnguloA) * MathF.Sin(AnguloB) * MathF.Sin(AnguloC) + k * MathF.Cos(AnguloA) * MathF.Sin(AnguloB) * MathF.Sin(AnguloC) -
               i * MathF.Cos(AnguloB) * MathF.Sin(AnguloC);
    }

    static float CalcularZ(int i, int j, int k)
    {
        return k * MathF.Cos(AnguloA) * MathF.Cos(AnguloB) - j * MathF.Sin(AnguloA) * MathF.Cos(AnguloB) + i * MathF.Sin(AnguloB);
    }

    static void CalcularParaSuperficie(float cuboX, float cuboY, float cuboZ, char caractere)
    {
        coordX = CalcularX((int)cuboX, (int)cuboY, (int)cuboZ);
        coordY = CalcularY((int)cuboX, (int)cuboY, (int)cuboZ);
        coordZ = CalcularZ((int)cuboX, (int)cuboY, (int)cuboZ) + distanciaDaCamera;

        inversoZ = 1 / coordZ;

        posX = (int)(larguraTela / 2 + deslocamentoHorizontal + fatorEscala * inversoZ * coordX * 2);
        posY = (int)(alturaTela / 2 + fatorEscala * inversoZ * coordY);

        indice = posX + posY * larguraTela;
        if (indice >= 0 && indice < larguraTela * alturaTela)
        {
            if (inversoZ > bufferProfundidade[indice])
            {
                bufferProfundidade[indice] = inversoZ;
                bufferTela[indice] = caractere;
            }
        }
    }

    static void DesenharCubo()
    {
        for (float cuboX = -larguraCubo; cuboX < larguraCubo; cuboX += velocidadeIncremento)
        {
            for (float cuboY = -larguraCubo; cuboY < larguraCubo; cuboY += velocidadeIncremento)
            {
                CalcularParaSuperficie(cuboX, cuboY, -larguraCubo, '@');
                CalcularParaSuperficie(larguraCubo, cuboY, cuboX, '$');
                CalcularParaSuperficie(-larguraCubo, cuboY, -cuboX, '~');
                CalcularParaSuperficie(-cuboX, cuboY, larguraCubo, '#');
                CalcularParaSuperficie(cuboX, -larguraCubo, -cuboY, ';');
                CalcularParaSuperficie(cuboX, larguraCubo, cuboY, '+');
            }
        }
    }

    static void DesenharTriangulo()
    {
        float baseTriangulo = 20;
        for (float y = -baseTriangulo; y <= baseTriangulo; y += velocidadeIncremento)
        {
            float x = baseTriangulo - Math.Abs(y); // Calcular a largura do triângulo
            CalcularParaSuperficie(-x, y, 0, '*');
            CalcularParaSuperficie(x, y, 0, '*');
        }
    }

    static void Main()
    {
        Console.Clear();
        while (true)
        {
            // Preenche os buffers com os valores padrões
            Array.Fill(bufferTela, caractereFundo);
            Array.Fill(bufferProfundidade, 0f);
            larguraCubo = 20;
            deslocamentoHorizontal = -2 * larguraCubo;

            // Desenha o cubo ou o triângulo, dependendo do ciclo
            if (primeiroCiclo)
            {
                DesenharCubo();

                // Verifica se completou uma volta
                if (AnguloA >= 2 * MathF.PI) // Um ciclo completo de rotação
                {
                    primeiroCiclo = false; // Muda para o triângulo
                }
            }
            else
            {
                DesenharTriangulo(); // Desenha o triângulo
            }

            // Limpa e desenha o buffer
            Console.SetCursorPosition(0, 0);
            for (int k = 0; k < larguraTela * alturaTela; k++)
            {
                Console.Write(k % larguraTela == 0 ? '\n' : bufferTela[k]);
            }

            // Incrementa os ângulos de rotação
            AnguloA += 0.05f;
            AnguloB += 0.05f;
            AnguloC += 0.01f;

            // Pausa para simular o atraso (usleep em C)
            Thread.Sleep(16);  // 60 FPS = ~16ms por frame
        }
    }
}
