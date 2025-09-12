namespace Latte.Core.Type;


public static class ColorGenerator
{
    public static ColorRGBA FromIndex(uint index, int step = 1)
    {
        int[] color = [255, 0, 0];
        uint channelIndex = 1;

        var decreasePrevious = false;

        for (uint i = 0; i < index; i++)
        {
            var realChannelIndex = RelativateChannelIndex(channelIndex);
            ref var channel = ref color[realChannelIndex];
            ref var previousChannel = ref color[GetPreviousChannelIndex(realChannelIndex)];

            if (decreasePrevious)
                previousChannel -= step;
            else
                channel += step;

            if (decreasePrevious && previousChannel.IsChannelMined())
            {
                decreasePrevious = false;
                previousChannel = 0;
                channelIndex++;
            }

            else if (channel.IsChannelMaxed())
            {
                channel = 255;
                decreasePrevious = true;
            }
        }

        return new ColorRGBA((byte)color[0], (byte)color[1], (byte)color[2]);
    }

    private static bool IsChannelMaxed(this int channel) => channel >= 255;
    private static bool IsChannelMined(this int channel) => channel <= 0;

    private static uint RelativateChannelIndex(uint index)
    {
        while (index >= 3)
            index -= 3;

        return index;
    }

    private static uint GetNextChannelIndex(uint index) => index >= 2 ? 0 : index + 1;
    private static uint GetPreviousChannelIndex(uint index) => index == 0 ? 2 : index - 1;
}
