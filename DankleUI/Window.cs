﻿using Assembler;
using Dankle;
using Dankle.Components;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Vulkan.Xlib;

namespace DankleUI
{
	public class Window
	{
		private readonly Sdl2Window SDL2;
		private readonly GraphicsDevice GD;
		private readonly CommandList CMDs;
		private readonly ImGuiRenderer ImGuiRenderer;

		public Computer? Computer;
		public int MemSize = 2048;
		public string LastError = "";
		public Dictionary<Guid, ComponentState> ComponentStates = [];

		public Window()
		{
			VeldridStartup.CreateWindowAndGraphicsDevice(new(5, 5, 1920, 1080, WindowState.Normal, "Dankle"), out SDL2, out GD);
			ImGuiRenderer = new(GD, GD.MainSwapchain.Framebuffer.OutputDescription, (int)GD.MainSwapchain.Framebuffer.Width, (int)GD.MainSwapchain.Framebuffer.Height);
			CMDs = GD.ResourceFactory.CreateCommandList();
		}

		private void Draw()
		{
			var font = ImGui.GetFont();
			font.FontSize = 18;
			ImGui.PushFont(font);

			ImGui.Begin("Assembly");
			ImGui.InputTextMultiline("Program", ref Program, 696969, ImGui.GetWindowSize(), ImGuiInputTextFlags.AllowTabInput);
			ImGui.End();

			ImGui.Begin("Computer");

			ImGui.InputInt("Memory Size", ref MemSize);
			if (ImGui.Button("Run")) CreateComputer();
			if (Computer is not null && !Computer.StoppingOrStopped)
			{
				ImGui.SameLine();
				if (ImGui.Button("Stop")) Computer.Stop();
			}
			if (LastError != "") ImGui.TextColored(new(1, 0, 0, 1), "Error: " + LastError);

			ImGui.End();

			if (Computer is not null)
			{
				foreach (var i in Computer.GetComponents())
				{
					if (i is BufferTerminal term) DrawTerminal(term);
					if (i is CPUCore core) DrawCore(core);
				}
			}

			ImGui.PopFont();
		}

		private void DrawCore(CPUCore core)
		{
			var state = ComponentStates[core.ID];
			ImGui.Begin("Core");

			ImGui.Text($"Clockspeed: {core.Clockspeed:###.##} MHz");
			ImGui.Checkbox("Query Core (may affect performance)", ref state.GetInfo);

			if (state.GetInfo)
			{
				ImGui.Text(core.GetDump());
			}

			ImGui.End();
		}

		private static void DrawTerminal(BufferTerminal term)
		{
			ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1));
			ImGui.Begin("Terminal");

			ImGui.TextWrapped(term.Buffer);

			ImGui.End();
			ImGui.PopStyleColor();
		}

		private void CreateComputer()
		{
			if (Computer is not null && Computer.Started) Computer.Stop();

			LastError = "";
			try
			{
				var tokenizer = new Tokenizer(Program + "\n" + BIOS);
				var parser = new Parser(tokenizer.Parse(), (uint)MemSize);
				parser.Parse();

				var data = parser.GetBinary();

				Computer = new((uint)(MemSize + data.Length + 1024));
				Computer.AddComponent<BufferTerminal>(0xFFFFFFF0);
				Computer.AddComponent<Dankle.Components.Timer>(0xFFFFFFF1);
				Computer.WriteMem((uint)MemSize, data);
				Computer.GetComponent<CPUCore>().ProgramCounter = (uint)MemSize;

				Computer.Debug = true;

				ComponentStates.Clear();
				foreach (var i in Computer.GetComponents())
				{
					ComponentStates[i.ID] = new();
				}

				Computer.Run(false);
			}
			catch (Exception e)
			{
				LastError = e.Message;
				Computer = null;
			}
		}

		public void Run()
		{
			var time1 = DateTime.Now.Ticks;
			long time2;
			while (SDL2.Exists)
			{
				var input = SDL2.PumpEvents();
				if (!SDL2.Exists) { break; }

				time2 = DateTime.Now.Ticks;
				ImGuiRenderer.Update((time2 - time1) / 10000000f, input);

				Draw();

				CMDs.Begin();
				CMDs.SetFramebuffer(GD.MainSwapchain.Framebuffer);
				CMDs.ClearColorTarget(0, RgbaFloat.Black);
				ImGuiRenderer.Render(GD, CMDs);
				CMDs.End();
				GD.SubmitCommands(CMDs);
				GD.SwapBuffers();

				time1 = time2;
			}
		}

		public string Program = @"main:
	ld r0, text
	call write
	hlt

text: ""ojifdoijfufdguihfd""
";

		public string BIOS = @"
write:
	push r0
	push r1
_write_loop:
	ldb r1, [r0]
	cmp r1, 0
	je _write_end
	stb [0xFFFFFFF0], r1
	inc r0
	jmp _write_loop
_write_end:
	pop r1
	pop r0
	ret
";
	}

	public class ComponentState
	{
		// CPUCore
		public bool GetInfo = true;
	}
}
