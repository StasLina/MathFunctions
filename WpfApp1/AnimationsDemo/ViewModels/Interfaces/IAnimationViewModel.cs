// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnimationViewModel.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace AnimationsDemo
{
    using System;
    using System.Threading.Tasks;

    public interface IAnimationViewModel
    {
        bool SupportsEasingFunction { get; }
        bool IsAnimationRuning { get; set; }
        Task AnimateAsync(CancellationToken cancellationToken = new CancellationToken());
        Task AnimateAsync(CancellationToken cancellationToken, AnimationSettings animationSettings);
    }
}