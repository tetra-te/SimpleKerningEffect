﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;
using System.Collections.Immutable;

namespace SimpleKerningEffect.ForVideoEffectChain
{
    public class VideoEffectChainNode
    {
        readonly IGraphicsDevicesAndContext devices;
        readonly AffineTransform2D transform;
        readonly ID2D1Bitmap empty;
        readonly FrameAndLength fl = new();
        ID2D1Image? input;
        bool isEmpty;
        List<(IVideoEffect effect, IVideoEffectProcessor processor)> Chain = [];
        TimelineItemSourceDescription? lastTimelineSourceDescription;
        DrawDescription? lastDrawDescription;

        ID2D1Image output;
        public ID2D1Image Output => isEmpty ? empty : output;

        public VideoEffectChainNode(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            transform = new AffineTransform2D(devices.DeviceContext);
            output = transform.Output;

            empty = devices.DeviceContext.CreateEmptyBitmap();
            
            isEmpty = true;
        }

        public void UpdateChain(ImmutableList<IVideoEffect> effects)
        {
            var disposedIndex = from tuple in Chain
                                where !effects.Contains(tuple.effect)
                                select Chain.IndexOf(tuple) into i
                                orderby i descending
                                select i;
            foreach (int index in disposedIndex)
            {
                IVideoEffectProcessor processor = Chain[index].processor;
                processor.ClearInput();
                processor.Dispose();
                Chain.RemoveAt(index);
            }

            List<IVideoEffect> keeped = Chain.Select((e_ep) => e_ep.effect).ToList();
            List<(IVideoEffect effect, IVideoEffectProcessor processor)> newChain = new(effects.Count);
            foreach (var effect in effects)
            {
                int index = keeped.IndexOf(effect);
                newChain.Add(index < 0 ? (effect, effect.CreateVideoEffect(devices)) : Chain[index]);
            }

            Chain = newChain;
        }

        public void SetInputAndEffects(ID2D1Image? input, ImmutableList<IVideoEffect> effects)
        {
            this.input = input;
            if (input == null)
            {
                isEmpty = true;
                return;
            }
            else
            {
                if (effects.Count > 0)
                {
                    UpdateChain(effects);

                    if (lastTimelineSourceDescription is not null && lastDrawDescription is not null)
                        UpdateOutputAndDescription(lastTimelineSourceDescription, lastDrawDescription);
                    else
                        transform.SetInput(0, input, true);
                }
                else
                {
                    transform.SetInput(0, input, true);
                }
                isEmpty = false;
            }
        }

        public void ClearChain()
        {
            foreach (var (_, processor) in Chain)
            {
                processor.ClearInput();
                processor.Dispose();
            }
            Chain.Clear();
            transform.SetInput(0, input, true);
        }

        public void ClearInput()
        {
            input = null;
            transform.SetInput(0, null, true);
            foreach (var (_, processor) in Chain)
            {
                processor.ClearInput();
                processor.Dispose();
            }
            Chain.Clear();
        }

        public DrawDescription UpdateOutputAndDescription(TimelineItemSourceDescription timelineSourceDescription, DrawDescription drawDescription)
        {
            lastTimelineSourceDescription = timelineSourceDescription;
            lastDrawDescription = drawDescription;

            if (input == null)
            {
                isEmpty = true;
                return drawDescription;
            }

            fl.UpdateFrom(timelineSourceDescription);
            DrawDescription desc = new(
                drawDescription.Draw,
                drawDescription.CenterPoint,
                drawDescription.Zoom,
                drawDescription.Rotation,
                drawDescription.Camera,
                drawDescription.ZoomInterpolationMode,
                drawDescription.Opacity,
                drawDescription.Invert,
                drawDescription.Controllers
                );

            ID2D1Image? image = input;
            foreach (var (effect, processor) in Chain)
            {
                if (effect.IsEnabled)
                {
                    IVideoEffectProcessor item = processor;
                    item.SetInput(image);
                    TimelineItemSourceDescription timeLineItemSourceDescription
                        = new(timelineSourceDescription, fl.Frame, fl.Length, 0);
                    EffectDescription effectDescription = new(timeLineItemSourceDescription, desc, 0, 1);
                    desc = item.Update(effectDescription);
                    image = item.Output;
                }
            }

            transform.SetInput(0, image, true);
            isEmpty = false;
            return desc;
        }

        public void Dispose()
        {
            transform.SetInput(0, null, true);
            transform.Dispose();
            empty.Dispose();
            output.Dispose();

            Chain.ForEach(i =>
            {
                i.processor.ClearInput();
                i.processor.Dispose();
            });
        }
    }
}
