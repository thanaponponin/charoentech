const App = (() => {
  'use strict'

  // Debounced resize event (width only). [ref: https://paulbrowne.xyz/debouncing]
  function resize(a, b) {
    const c = [window.innerWidth]
    return window.addEventListener('resize', () => {
      const e = c.length
      c.push(window.innerWidth)
      if (c[e] !== c[e - 1]) {
        clearTimeout(b)
        b = setTimeout(a, 150)
      }
    }), a
  }

  // Bootstrap breakPoint checker
  function breakPoint(value) {
    let el, check, cls

    switch (value) {
      case 'xs': cls = 'd-none d-sm-block'; break
      case 'sm': cls = 'd-block d-sm-none d-md-block'; break
      case 'md': cls = 'd-block d-md-none d-lg-block'; break
      case 'lg': cls = 'd-block d-lg-none d-xl-block'; break
      case 'xl': cls = 'd-block d-xl-none'; break
      case 'smDown': cls = 'd-none d-md-block'; break
      case 'mdDown': cls = 'd-none d-lg-block'; break
      case 'lgDown': cls = 'd-none d-xl-block'; break
      case 'smUp': cls = 'd-block d-sm-none'; break
      case 'mdUp': cls = 'd-block d-md-none'; break
      case 'lgUp': cls = 'd-block d-lg-none'; break
    }

    el = document.createElement('div')
    el.setAttribute('class', cls)
    document.body.appendChild(el)
    check = el.offsetParent === null
    el.parentNode.removeChild(el)

    return check
  }

  // Shorthand for Bootstrap breakPoint checker
  function xs() { return breakPoint('xs') }
  function sm() { return breakPoint('sm') }
  function md() { return breakPoint('md') }
  function lg() { return breakPoint('lg') }
  function xl() { return breakPoint('xl') }
  function smDown() { return breakPoint('smDown') }
  function mdDown() { return breakPoint('mdDown') }
  function lgDown() { return breakPoint('lgDown') }
  function smUp() { return breakPoint('smUp') }
  function mdUp() { return breakPoint('mdUp') }
  function lgUp() { return breakPoint('lgUp') }

  // Show filename for bootstrap custom file input
  function customFileInput() {
    for (const el of document.querySelectorAll('.custom-file > input[type="file"]')) {
      el.addEventListener('change', () => {
        const fileLength = el.files.length
        let filename = fileLength ? el.files[0].name : 'Choose file'
        filename = fileLength > 1 ? fileLength + ' files' : filename // if more than one file, show '{amount} files'
        el.parentElement.querySelector('label').textContent = filename
      })
    }
  }

  // Dropdown hover
  function dropdownHover() {
    for (const el of document.querySelectorAll('.dropdown-hover')) {
      el.addEventListener('mouseenter', () => {
        lgUp() && el.classList.add('show')
      })
      el.addEventListener('mouseleave', () => {
        lgUp() && el.classList.remove('show')
      })
    }
  }

  // Background cover
  function backgroundCover() {
    for (const el of document.querySelectorAll('[data-cover]')) {
      el.style.backgroundImage = `url(${el.getAttribute('data-cover')})`
    }
  }

  // Spinner input
  function customSpinner() {
    for (const el of document.querySelectorAll('.custom-spinner')) {
      const input = el.querySelector('input[type="number"]')
      const min = input.getAttribute('min')
      const max = input.getAttribute('max')
      const up = el.querySelector('.up')
      const down = el.querySelector('.down')
      up.addEventListener('click', () => {
        if (input.value < max) {
          input.value = parseInt(input.value) + 1
          input.dispatchEvent(new Event('change'))
        }
      })
      down.addEventListener('click', () => {
        if (input.value > min) {
          input.value = parseInt(input.value) - 1
          input.dispatchEvent(new Event('change'))
        }
      })
    }
  }

  // Sticky Header
  function stickyHeader() {
    if (document.querySelector('header')) {
      const header = document.querySelector('header')
      header.insertAdjacentHTML('beforebegin', '<div id="tmpWrapper"></div>')
      const wrapper = document.getElementById('tmpWrapper')
      const offsetTop = wrapper.offsetTop
      const fixedCls = 'fixed-top'
      let lastScroll = window.pageYOffset
      window.addEventListener('scroll', () => {
        const headerHeight = header.offsetHeight
        let scrollTop = window.pageYOffset
        if (scrollTop < lastScroll && scrollTop <= offsetTop) {
          header.classList.contains(fixedCls) && header.classList.remove(fixedCls)
          wrapper.style.height = 0
        } else if (scrollTop >= offsetTop + headerHeight + 20) {
          wrapper.style.height = headerHeight + 'px'
          header.classList.add(fixedCls)
        }
        lastScroll = scrollTop
      })
    }
  }

  // Redirect forwardable dropdown toggle
  function forwardable() {
    for (const el of document.querySelectorAll('.forwardable')) {
      el.addEventListener('click', () => {
        $(el).dropdown('toggle') // prevent dropdown showing
        location.href = el.href
      })
    }
  }

  // Responsive height
  function responsiveHeight() {
    for (const el of document.querySelectorAll('[data-height]')) {
      const h = el.getAttribute('data-height').split(' ')
      if (h.length > 1) {
        xs() && (el.style.height = h[0])
        sm() && (el.style.height = h[1])
        md() && (el.style.height = h[2])
        lg() && (el.style.height = h[3])
        xl() && (el.style.height = h[4])
      } else {
        el.style.height = h[0]
      }
    }
  }

  // Responsive width
  function responsiveWidth() {
    for (const el of document.querySelectorAll('[data-width]')) {
      const h = el.getAttribute('data-width').split(' ')
      if (h.length > 1) {
        xs() && (el.style.width = h[0])
        sm() && (el.style.width = h[1])
        md() && (el.style.width = h[2])
        lg() && (el.style.width = h[3])
        xl() && (el.style.width = h[4])
      } else {
        el.style.width = h[0]
      }
    }
  }

  return {
    init: () => {
      customFileInput()
      dropdownHover()
      backgroundCover()
      customSpinner()
      stickyHeader()
      forwardable()
      resize(() => {
        responsiveHeight()
        responsiveWidth()
      })()
    },
    resize: callback => resize(callback),
    xs: () => xs(),
    sm: () => sm(),
    md: () => md(),
    lg: () => lg(),
    xl: () => xl(),
    smDown: () => smDown(),
    mdDown: () => mdDown(),
    lgDown: () => lgDown(),
    smUp: () => smUp(),
    mdUp: () => mdUp(),
    lgUp: () => lgUp(),
  }
})()

// This is for development, attach breakpoint to document title
/* App.resize(() => {
  if (App.xs()) { document.title = 'xs' }
  if (App.sm()) { document.title = 'sm' }
  if (App.md()) { document.title = 'md' }
  if (App.lg()) { document.title = 'lg' }
  if (App.xl()) { document.title = 'xl' }
})() */